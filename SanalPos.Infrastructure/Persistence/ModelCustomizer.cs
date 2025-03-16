using Microsoft.EntityFrameworkCore;
using SanalPos.Application.Common.Interfaces;

namespace SanalPos.Infrastructure.Persistence
{
    public class ModelCustomizer : IModelCustomizer
    {
        public void Customize(ModelBuilder modelBuilder)
        {
            // Order entity konfigürasyonu
            modelBuilder.Entity<Domain.Entities.Order>(entity =>
            {
                entity.Property(e => e.Discount)
                    .HasColumnType("decimal(18,2)");
                
                entity.Property(e => e.Tax)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.TotalAmount)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
            });

            // Product entity konfigürasyonu
            modelBuilder.Entity<Domain.Entities.Product>(entity =>
            {
                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
                
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsRequired();
                
                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .IsRequired();
                
                // CategoryId özelliğinin veritabanında sütun olarak yer almamasını sağla
                entity.Ignore(e => e.CategoryId);
            });

            // Table entity konfigürasyonu
            modelBuilder.Entity<Domain.Entities.Table>(entity =>
            {
                entity.Property(e => e.Number)
                    .HasMaxLength(20)
                    .IsRequired();
                
                entity.Property(e => e.Description)
                    .HasMaxLength(200);
                
                entity.Property(e => e.QrCode)
                    .HasMaxLength(255);
                
                entity.HasOne(e => e.CurrentOrder)
                    .WithMany()
                    .HasForeignKey(e => e.CurrentOrderId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // OrderItem entity konfigürasyonu
            modelBuilder.Entity<Domain.Entities.OrderItem>(entity =>
            {
                entity.Property(e => e.UnitPrice)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
            });
        }
    }
} 