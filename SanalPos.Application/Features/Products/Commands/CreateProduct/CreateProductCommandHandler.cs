using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SanalPos.Application.Common.Interfaces;
using SanalPos.Domain.Entities;
using SanalPos.Domain.Exceptions;

namespace SanalPos.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<CreateProductCommandHandler> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;

        public CreateProductCommandHandler(
            IApplicationDbContext context,
            ILogger<CreateProductCommandHandler> logger,
            ICurrentUserService currentUserService,
            IDateTime dateTime)
        {
            _context = context;
            _logger = logger;
            _currentUserService = currentUserService;
            _dateTime = dateTime;
        }

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Yeni ürün oluşturuluyor: {ProductName}", request.Name);

            if (request.Price < 0)
            {
                throw new DomainException("Ürün fiyatı negatif olamaz.");
            }

            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                ImageUrl = request.ImageUrl,
                ProductCategoryId = request.ProductCategoryId,
                IsActive = true,
                IsAvailable = true,
                CreatedBy = _currentUserService.UserId?.ToString() ?? "system",
                LastModifiedBy = _currentUserService.UserId?.ToString() ?? "system"
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Ürün başarıyla oluşturuldu. Ürün ID: {ProductId}", product.Id);

            return product.Id;
        }
    }
} 