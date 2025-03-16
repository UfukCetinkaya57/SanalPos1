using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SanalPos.Domain.Entities;
using SanalPos.Domain.Enums;

namespace SanalPos.Infrastructure.Persistence.Configurations
{
    public class TableConfiguration : IEntityTypeConfiguration<Table>
    {
        public void Configure(EntityTypeBuilder<Table> builder)
        {
            // Tablo adını açıkça belirt
            builder.ToTable("Tables", schema: "dbo");
            
            // Primary key tanımla
            builder.HasKey(t => t.Id);
            
            // Özellikleri yapılandır
            builder.Property(t => t.Number)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.Description)
                .HasMaxLength(200);

            builder.Property(t => t.Capacity)
                .IsRequired();

            builder.Property(t => t.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(t => t.Location)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(t => t.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            // CreatedBy alanı için yapılandırma
            builder.Property(t => t.CreatedBy)
                .IsRequired(false)
                .HasMaxLength(100);
                
            // LastModifiedBy alanı için yapılandırma
            builder.Property(t => t.LastModifiedBy)
                .HasMaxLength(100)
                .IsRequired(false);

            // CreatedAt ve LastModifiedAt alanları yapılandırması
            builder.Property(t => t.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");
                
            builder.Property(t => t.LastModifiedAt)
                .IsRequired(false);

            // OrderId'nin yabancı anahtar olarak ilişkisi
            builder.Property(t => t.OrderId)
                .IsRequired(false);
                
            // CurrentOrder ilişkisini tanımla
            builder.HasOne(t => t.CurrentOrder)
                .WithMany()
                .HasForeignKey(t => t.CurrentOrderId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
} 