using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SanalPos.Application.Authentication.Common;
using SanalPos.Application.Common.Interfaces;
using SanalPos.Domain.Entities;
using SanalPos.Domain.Enums;

namespace SanalPos.Application.Authentication.Commands.Register
{
    public class RegisterCommand : IRequest<AuthenticationResult>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        private readonly IApplicationDbContext _context;

        public RegisterCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(v => v.FirstName)
                .NotEmpty().WithMessage("Ad boş olamaz.")
                .MaximumLength(50).WithMessage("Ad 50 karakterden uzun olamaz.");

            RuleFor(v => v.LastName)
                .NotEmpty().WithMessage("Soyad boş olamaz.")
                .MaximumLength(50).WithMessage("Soyad 50 karakterden uzun olamaz.");

            RuleFor(v => v.Email)
                .NotEmpty().WithMessage("E-posta adresi boş olamaz.")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.")
                .MustAsync(BeUniqueEmail).WithMessage("Bu e-posta adresi zaten kullanılıyor.");

            RuleFor(v => v.Password)
                .NotEmpty().WithMessage("Şifre boş olamaz.")
                .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.")
                .Matches("[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir.")
                .Matches("[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir.")
                .Matches("[0-9]").WithMessage("Şifre en az bir rakam içermelidir.");
        }

        private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
        {
            return await _context.Users
                .AllAsync(u => u.Email != email, cancellationToken);
        }
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthenticationResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public RegisterCommandHandler(
            IApplicationDbContext context,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<AuthenticationResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var passwordHash = _passwordHasher.HashPassword(request.Password);

            var user = new User(
                email: request.Email,
                username: request.Email,
                passwordHash: passwordHash,
                role: "User",
                firstName: request.FirstName,
                lastName: request.LastName);

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            var token = _jwtTokenGenerator.GenerateToken(user);

            return new AuthenticationResult(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                token);
        }
    }
} 