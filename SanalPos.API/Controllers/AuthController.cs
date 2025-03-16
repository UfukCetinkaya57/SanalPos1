using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SanalPos.Application.Authentication.Models;
using SanalPos.Application.Common.Interfaces;
using SanalPos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SanalPos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IApplicationDbContext _context;

        public AuthController(
            IJwtService jwtService,
            IPasswordHasher<User> passwordHasher,
            IApplicationDbContext context)
        {
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
            _context = context;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            
            if (user == null)
            {
                return Unauthorized(new { message = "Geçersiz email veya şifre" });
            }

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new { message = "Geçersiz email veya şifre" });
            }

            var token = _jwtService.GenerateToken(user);

            return Ok(new { token });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest(new { message = "Bu email adresi zaten kullanılıyor" });
            }

            var user = new User(
                email: request.Email,
                username: request.Email,
                passwordHash: _passwordHasher.HashPassword(null, request.Password),
                role: "User",
                firstName: request.FirstName,
                lastName: request.LastName,
                phoneNumber: request.PhoneNumber);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _jwtService.GenerateToken(user);

            return Ok(new { token });
        }

        [HttpPost("register-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest(new { message = "Bu email adresi zaten kullanılıyor" });
            }

            var user = new User(
                email: request.Email,
                username: request.Email,
                passwordHash: _passwordHasher.HashPassword(null, request.Password),
                role: "Admin",
                firstName: request.FirstName,
                lastName: request.LastName,
                phoneNumber: request.PhoneNumber);

            user.CreatedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Admin kullanıcı başarıyla oluşturuldu" });
        }
    }
} 