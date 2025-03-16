using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SanalPos.Application.Common.Interfaces;
using SanalPos.Domain.Entities;

namespace SanalPos.Application.Products.Commands.CreateProduct
{
    public class CreateProductCommand : IRequest<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public Guid CategoryId { get; set; }
        public int PreparationTimeMinutes { get; set; }
    }

    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("Ürün adı boş olamaz.")
                .MaximumLength(100).WithMessage("Ürün adı 100 karakterden uzun olamaz.");

            RuleFor(v => v.Description)
                .MaximumLength(500).WithMessage("Açıklama 500 karakterden uzun olamaz.");

            RuleFor(v => v.Price)
                .GreaterThan(0).WithMessage("Fiyat sıfırdan büyük olmalıdır.");

            RuleFor(v => v.CategoryId)
                .NotEmpty().WithMessage("Kategori seçilmelidir.")
                .MustAsync(async (categoryId, cancellation) => 
                    await unitOfWork.ProductCategories.ExistsAsync(c => c.Id == categoryId))
                .WithMessage("Seçilen kategori bulunamadı.");

            RuleFor(v => v.PreparationTimeMinutes)
                .GreaterThan(0).WithMessage("Hazırlama süresi sıfırdan büyük olmalıdır.");
        }
    }

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product(
                request.Name,
                request.Description,
                request.Price,
                request.CategoryId,
                request.PreparationTimeMinutes,
                request.ImageUrl);

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return product.Id;
        }
    }
} 