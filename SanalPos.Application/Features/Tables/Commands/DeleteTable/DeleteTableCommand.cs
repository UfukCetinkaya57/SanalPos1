using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SanalPos.Application.Common.Exceptions;
using SanalPos.Application.Common.Interfaces;
using SanalPos.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace SanalPos.Application.Features.Tables.Commands.DeleteTable
{
    public class DeleteTableCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }

    public class DeleteTableCommandValidator : AbstractValidator<DeleteTableCommand>
    {
        public DeleteTableCommandValidator()
        {
            RuleFor(v => v.Id)
                .NotEmpty().WithMessage("Masa ID'si boş olamaz.");
        }
    }

    public class DeleteTableCommandHandler : IRequestHandler<DeleteTableCommand, Unit>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;

        public DeleteTableCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            IDateTime dateTime)
        {
            _context = context;
            _currentUserService = currentUserService;
            _dateTime = dateTime;
        }

        public async Task<Unit> Handle(DeleteTableCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Tables
                .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                throw new Exception($"Masa bulunamadı: {request.Id}");
            }

            // Silinmeden önce masanın aktif siparişi olup olmadığını kontrol et
            if (entity.OrderId != null)
            {
                throw new Exception("Aktif siparişi olan masa silinemez.");
            }

            _context.Tables.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
} 