using System;
using AutoMapper;
using SanalPos.Application.Common.Mappings;
using SanalPos.Domain.Entities;

namespace SanalPos.Application.Products.Queries.GetProducts
{
    public class ProductDto : IMapFrom<Product>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool IsAvailable { get; set; }
        public int PreparationTimeMinutes { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Product, ProductDto>()
                .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category.Name));
        }
    }
} 