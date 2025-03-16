using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SanalPos.Domain.Common;
using SanalPos.Domain.Entities;

namespace SanalPos.Infrastructure.Persistence.Configurations
{
    public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            builder.HasKey(pc => pc.Id);

            builder.Property(pc => pc.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(pc => pc.Description)
                .HasMaxLength(200);

            builder.Property(pc => pc.DisplayOrder)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(pc => pc.ImageUrl)
                .HasMaxLength(255);

            // Ürünler ile ilişki
            builder.HasMany(pc => pc.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // CreatedBy alanı için varsayılan değer
            builder.Property(pc => pc.CreatedBy)
                .IsRequired()
                .HasMaxLength(100);
                
            // LastModifiedBy alanı için varsayılan değer
            builder.Property(pc => pc.LastModifiedBy)
                .HasMaxLength(100);

            // Örnek kategori verileri
            var category1 = new ProductCategory("Yiyecekler", "Tüm yiyecek ürünleri", 1, "images/categories/food.jpg");
            var category2 = new ProductCategory("İçecekler", "Tüm içecek ürünleri", 2, "images/categories/drinks.jpg");
            var category3 = new ProductCategory("Tatlılar", "Tüm tatlı ürünleri", 3, "images/categories/desserts.jpg");
            
            // CreatedBy ve LastModifiedBy alanlarını ayarlama
            category1.SetCreatedBy("System");
            category1.SetLastModifiedBy("System");
            
            category2.SetCreatedBy("System");
            category2.SetLastModifiedBy("System");
            
            category3.SetCreatedBy("System");
            category3.SetLastModifiedBy("System");
            
            builder.HasData(category1, category2, category3);
        }
    }
} 