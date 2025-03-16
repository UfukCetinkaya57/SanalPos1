using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SanalPos.Application.Common.Interfaces;

namespace SanalPos.Application.Features.Tables.Commands.UpdateTable
{
    public class UpdateTableCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Number { get; set; } = null!;
        public string? Description { get; set; }
        public int Capacity { get; set; }
        public string? Location { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateTableCommandValidator : AbstractValidator<UpdateTableCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateTableCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(v => v.Id)
                .NotEmpty().WithMessage("Masa ID'si boş olamaz.");

            RuleFor(v => v.Number)
                .NotEmpty().WithMessage("Masa numarası boş olamaz.")
                .MaximumLength(20).WithMessage("Masa numarası en fazla 20 karakter olabilir.")
                .MustAsync(BeUniqueNumberExceptThis).WithMessage("Bu masa numarası zaten başka bir masa tarafından kullanılmaktadır.");

            RuleFor(v => v.Capacity)
                .GreaterThan(0).WithMessage("Kapasite sıfırdan büyük olmalıdır.");

            RuleFor(v => v.Description)
                .MaximumLength(200).WithMessage("Açıklama en fazla 200 karakter olabilir.");

            RuleFor(v => v.Location)
                .MaximumLength(200).WithMessage("Konum en fazla 200 karakter olabilir.");
        }

        private async Task<bool> BeUniqueNumberExceptThis(UpdateTableCommand command, string number, CancellationToken cancellationToken)
        {
            return await _context.Tables
                .AllAsync(t => t.Id == command.Id || t.Number != number, cancellationToken);
        }
    }

    public class UpdateTableCommandHandler : IRequestHandler<UpdateTableCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public UpdateTableCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateTableCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Tables
                .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                throw new Exception($"Masa bulunamadı: {request.Id}");
            }

            entity.Number = request.Number;
            entity.Description = request.Description;
            entity.Capacity = request.Capacity;
            entity.Location = request.Location;
            entity.IsActive = request.IsActive;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
} 