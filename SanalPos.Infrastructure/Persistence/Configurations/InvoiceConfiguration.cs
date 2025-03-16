using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SanalPos.Domain.Entities;

namespace SanalPos.Infrastructure.Persistence.Configurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.InvoiceNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(i => i.InvoiceNumber)
                .IsUnique();

            builder.Property(i => i.TotalAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.TaxAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.DiscountAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.IssueDate)
                .IsRequired();

            builder.Property(i => i.IsPaid)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(i => i.Notes)
                .HasMaxLength(500);

            // Sipariş ilişkisi
            builder.HasOne(i => i.Order)
                .WithMany()
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 