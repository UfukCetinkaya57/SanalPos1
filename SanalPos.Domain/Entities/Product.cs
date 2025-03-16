using System;
using SanalPos.Domain.Common;
using SanalPos.Domain.Exceptions;

namespace SanalPos.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; } = null!;
        public Guid ProductCategoryId { get; set; }
        public bool IsActive { get; set; }
        public bool IsAvailable { get; set; }
        public int PreparationTimeMinutes { get; set; }
        
        // ProductCategoryId için alias - EF Core ile uyumlu hale getirildi
        public Guid CategoryId { 
            get => ProductCategoryId; 
            set => ProductCategoryId = value; 
        }

        public virtual ProductCategory? Category { get; set; }

        public Product()
        {
            // EF Core için gerekli
        }

        public Product(string name, string description, decimal price, Guid categoryId, 
                      int preparationTimeMinutes, string imageUrl = null) : base()
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new DomainException("Ürün adı boş olamaz.");
            }
            
            if (price < 0)
            {
                throw new DomainException("Ürün fiyatı negatif olamaz.");
            }
            
            if (preparationTimeMinutes < 0)
            {
                throw new DomainException("Hazırlama süresi negatif olamaz.");
            }
            
            Name = name;
            Description = description;
            Price = price;
            ProductCategoryId = categoryId;
            ImageUrl = imageUrl;
            IsActive = true;
            StockQuantity = 0;
        }

        public void UpdateDetails(string name, string description, string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new DomainException("Ürün adı boş olamaz.");
            }
            
            Name = name;
            Description = description;
            ImageUrl = imageUrl;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice < 0)
            {
                throw new DomainException("Ürün fiyatı negatif olamaz.");
            }
            
            Price = newPrice;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void UpdateCategory(Guid newCategoryId)
        {
            ProductCategoryId = newCategoryId;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void UpdateStock(int quantity)
        {
            if (quantity < 0)
            {
                throw new ArgumentException("Stok miktarı negatif olamaz.", nameof(quantity));
            }

            StockQuantity = quantity;
        }

        public void DeductStock(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Azaltılacak miktar pozitif olmalıdır.", nameof(quantity));
            }

            if (StockQuantity < quantity)
            {
                throw new InvalidOperationException("Yeterli stok bulunmamaktadır.");
            }

            StockQuantity -= quantity;
        }

        public void AddStock(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Eklenecek miktar pozitif olmalıdır.", nameof(quantity));
            }

            StockQuantity += quantity;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void SetAvailability(bool isAvailable)
        {
            IsAvailable = isAvailable;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void UpdatePreparationTime(int minutes)
        {
            if (minutes < 0)
            {
                throw new DomainException("Hazırlama süresi negatif olamaz.");
            }

            PreparationTimeMinutes = minutes;
            LastModifiedAt = DateTime.UtcNow;
        }
    }
} 