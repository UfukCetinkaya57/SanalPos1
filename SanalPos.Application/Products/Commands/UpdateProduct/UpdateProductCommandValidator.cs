using FluentValidation;

namespace SanalPos.Application.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(v => v.Id)
                .NotEmpty().WithMessage("Ürün ID'si boş olamaz.");

            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("Ürün adı boş olamaz.")
                .MaximumLength(100).WithMessage("Ürün adı 100 karakterden uzun olamaz.");

            RuleFor(v => v.Description)
                .MaximumLength(500).WithMessage("Ürün açıklaması 500 karakterden uzun olamaz.");

            RuleFor(v => v.Price)
                .GreaterThan(0).WithMessage("Ürün fiyatı sıfırdan büyük olmalıdır.");

            RuleFor(v => v.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stok miktarı negatif olamaz.");

            RuleFor(v => v.ProductCategoryId)
                .NotEmpty().WithMessage("Ürün kategorisi seçilmelidir.");
        }
    }
} 