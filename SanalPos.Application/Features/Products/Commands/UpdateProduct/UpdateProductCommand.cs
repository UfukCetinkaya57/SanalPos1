using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using SanalPos.Application.Common.Exceptions;
using SanalPos.Application.Common.Interfaces;
using SanalPos.Domain.Entities;
using FluentValidation;

namespace SanalPos.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; } = null!;
        public Guid ProductCategoryId { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateProductCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(v => v.Id)
                .NotEmpty().WithMessage("Ürün ID'si boş olamaz.");

            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("Ürün adı boş olamaz.")
                .MaximumLength(100).WithMessage("Ürün adı 100 karakterden uzun olamaz.");

            RuleFor(v => v.Description)
                .MaximumLength(500).WithMessage("Açıklama 500 karakterden uzun olamaz.");

            RuleFor(v => v.Price)
                .GreaterThan(0).WithMessage("Fiyat sıfırdan büyük olmalıdır.");

            RuleFor(v => v.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stok miktarı negatif olamaz.");
        }
    }

    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public UpdateProductCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Products
                .FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Product), request.Id);
            }

            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.Price = request.Price;
            entity.StockQuantity = request.StockQuantity;
            entity.ImageUrl = request.ImageUrl;
            entity.ProductCategoryId = request.ProductCategoryId;
            entity.IsActive = request.IsActive;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
} 