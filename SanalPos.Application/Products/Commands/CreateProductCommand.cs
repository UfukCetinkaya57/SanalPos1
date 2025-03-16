using System;

namespace SanalPos.Application.Products.Commands
{
    public class CreateProductCommand
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; }
        public Guid ProductCategoryId { get; set; }
    }
} 