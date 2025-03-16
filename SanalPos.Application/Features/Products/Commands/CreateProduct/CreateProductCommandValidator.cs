using FluentValidation;
using System;

namespace SanalPos.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ürün adı zorunludur.")
                .MaximumLength(100).WithMessage("Ürün adı en fazla 100 karakter olmalıdır.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Ürün açıklaması zorunludur.")
                .MaximumLength(500).WithMessage("Ürün açıklaması en fazla 500 karakter olmalıdır.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Ürün fiyatı negatif olamaz.");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stok miktarı negatif olamaz.");

            RuleFor(x => x.ImageUrl)
                .NotEmpty().WithMessage("Ürün resmi URL'si zorunludur.")
                .MaximumLength(255).WithMessage("Ürün resmi URL'si en fazla 255 karakter olmalıdır.");

            RuleFor(x => x.ProductCategoryId)
                .NotEqual(Guid.Empty).WithMessage("Geçerli bir kategori seçilmelidir.");
        }
    }
} 