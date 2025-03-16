using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SanalPos.Application.Authentication.Common;
using SanalPos.Application.Common.Exceptions;
using SanalPos.Application.Common.Interfaces;
using SanalPos.Application.Common.Models;

namespace SanalPos.Application.Authentication.Commands.Login
{
    public class LoginCommand : IRequest<AuthenticationResult>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(v => v.Email)
                .NotEmpty().WithMessage("E-posta adresi boş olamaz.")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

            RuleFor(v => v.Password)
                .NotEmpty().WithMessage("Şifre boş olamaz.");
        }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthenticationResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginCommandHandler(
            IApplicationDbContext context,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<AuthenticationResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (user == null)
            {
                throw new ForbiddenAccessException("Geçersiz e-posta veya şifre.");
            }

            var isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                throw new ForbiddenAccessException("Geçersiz e-posta veya şifre.");
            }

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