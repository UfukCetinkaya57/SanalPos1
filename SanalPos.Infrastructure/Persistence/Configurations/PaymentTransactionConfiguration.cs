using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SanalPos.Domain.Entities;

namespace SanalPos.Infrastructure.Persistence.Configurations
{
    public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
    {
        public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
        {
            builder.HasKey(p => p.Id);
            
            builder.Property(p => p.TransactionNumber)
                .IsRequired()
                .HasMaxLength(50);
                
            builder.Property(p => p.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
                
            builder.Property(p => p.CardNumber)
                .HasMaxLength(20);
                
            builder.Property(p => p.CardHolderName)
                .HasMaxLength(100);
                
            builder.Property(p => p.AuthorizationCode)
                .HasMaxLength(50);
                
            builder.Property(p => p.ReferenceNumber)
                .HasMaxLength(50);
                
            builder.Property(p => p.ResponseCode)
                .HasMaxLength(10);
                
            builder.Property(p => p.ResponseMessage)
                .HasMaxLength(250);
                
            builder.Property(p => p.RefundReferenceNumber)
                .HasMaxLength(50);
                
            builder.HasOne(p => p.Order)
                .WithMany()
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 