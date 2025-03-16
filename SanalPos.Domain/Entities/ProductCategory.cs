using System;
using System.Collections.Generic;
using SanalPos.Domain.Common;
using SanalPos.Domain.Exceptions;

namespace SanalPos.Domain.Entities
{
    public class ProductCategory : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

        public ProductCategory()
        {
            // EF Core için gerekli
        }

        public ProductCategory(string name, string description = null, int displayOrder = 0, string imageUrl = null) : base()
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new DomainException("Kategori adı boş olamaz.");
            }
            
            Name = name;
            Description = description;
            DisplayOrder = displayOrder;
            ImageUrl = imageUrl;
            Products = new List<Product>();
        }

        public void SetCreatedBy(string createdBy)
        {
            if (string.IsNullOrWhiteSpace(createdBy))
            {
                throw new DomainException("Oluşturan kullanıcı bilgisi boş olamaz.");
            }
            
            CreatedBy = createdBy;
        }

        public void SetLastModifiedBy(string lastModifiedBy)
        {
            LastModifiedBy = lastModifiedBy;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void UpdateDetails(string name, string description, string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new DomainException("Kategori adı boş olamaz.");
            }
            
            Name = name;
            Description = description;
            ImageUrl = imageUrl;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void UpdateDisplayOrder(int order)
        {
            if (order < 0)
            {
                throw new ArgumentException("Görüntüleme sırası negatif olamaz.", nameof(order));
            }

            DisplayOrder = order;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
} 