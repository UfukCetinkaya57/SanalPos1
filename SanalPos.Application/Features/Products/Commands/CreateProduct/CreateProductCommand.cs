using System;
using MediatR;

namespace SanalPos.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommand : IRequest<Guid>
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; } = null!;
        public Guid ProductCategoryId { get; set; }
    }
} 