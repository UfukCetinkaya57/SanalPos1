using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SanalPos.Application.Common.Interfaces;
using SanalPos.Domain.Entities;
using SanalPos.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace SanalPos.Application.Features.Tables.Commands.CreateTable
{
    public class CreateTableCommand : IRequest<Guid>
    {
        public string Number { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Capacity { get; set; }
        public string Location { get; set; } = string.Empty;
    }

    public class CreateTableCommandValidator : AbstractValidator<CreateTableCommand>
    {
        private readonly IApplicationDbContext _context;

        public CreateTableCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(v => v.Number)
                .NotEmpty().WithMessage("Masa numarası gereklidir.")
                .MaximumLength(50).WithMessage("Masa numarası 50 karakterden uzun olamaz.")
                .MustAsync(BeUniqueNumber).WithMessage("Belirtilen masa numarası zaten kullanımda.");

            RuleFor(v => v.Capacity)
                .GreaterThan(0).WithMessage("Kapasite 0'dan büyük olmalıdır.");

            RuleFor(v => v.Location)
                .NotEmpty().WithMessage("Konum gereklidir.")
                .MaximumLength(100).WithMessage("Konum 100 karakterden uzun olamaz.");
        }

        private async Task<bool> BeUniqueNumber(string number, CancellationToken cancellationToken)
        {
            return await _context.Tables.AllAsync(t => t.Number != number, cancellationToken);
        }
    }

    public class CreateTableCommandHandler : IRequestHandler<CreateTableCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public CreateTableCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Guid> Handle(CreateTableCommand request, CancellationToken cancellationToken)
        {
            var entity = new Table(request.Number, request.Capacity, request.Description)
            {
                Status = TableStatus.Available,
                Location = request.Location,
                IsActive = true,
                CreatedBy = _currentUserService.UserId.HasValue ? _currentUserService.UserId.Value.ToString() : "System",
                LastModifiedBy = _currentUserService.UserId.HasValue ? _currentUserService.UserId.Value.ToString() : null,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };

            _context.Tables.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
} 