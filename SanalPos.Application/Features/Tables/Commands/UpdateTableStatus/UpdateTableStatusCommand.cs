using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SanalPos.Application.Common.Interfaces;
using SanalPos.Domain.Enums;

namespace SanalPos.Application.Features.Tables.Commands.UpdateTableStatus
{
    public class UpdateTableStatusCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public TableStatus Status { get; set; }
        public Guid? OrderId { get; set; }
    }

    public class UpdateTableStatusCommandValidator : AbstractValidator<UpdateTableStatusCommand>
    {
        public UpdateTableStatusCommandValidator()
        {
            RuleFor(v => v.Status)
                .IsInEnum().WithMessage("Geçersiz masa durumu.");
                
            RuleFor(v => v.OrderId)
                .NotEmpty()
                .When(v => v.Status == TableStatus.Occupied)
                .WithMessage("Masa meşgul olduğunda sipariş ID'si gereklidir.");
        }
    }

    public class UpdateTableStatusCommandHandler : IRequestHandler<UpdateTableStatusCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public UpdateTableStatusCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateTableStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Tables
                .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                throw new Exception($"Masa bulunamadı: {request.Id}");
            }

            entity.Status = request.Status;
            
            // Masa meşgulse OrderId'yi ayarla, değilse temizle
            if (request.Status == TableStatus.Occupied)
            {
                entity.OrderId = request.OrderId;
            }
            else
            {
                entity.OrderId = null;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
} 