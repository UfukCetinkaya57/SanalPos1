using System;
using SanalPos.Domain.Common;
using SanalPos.Domain.Exceptions;

namespace SanalPos.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? Notes { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;
        public bool IsCompleted { get; set; }

        public virtual Order? Order { get; set; }
        public virtual Product? Product { get; set; }

        public OrderItem()
        {
            // EF Core için gerekli
        }

        public OrderItem(Guid orderId, Guid productId, int quantity, decimal unitPrice, string notes = null) : base()
        {
            if (quantity <= 0)
            {
                throw new DomainException("Sipariş kalemi miktarı sıfırdan büyük olmalıdır.");
            }
            
            if (unitPrice < 0)
            {
                throw new DomainException("Birim fiyat negatif olamaz.");
            }
            
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
            Notes = notes;
            IsCompleted = false;
        }
        
        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
            {
                throw new ArgumentException("Miktar 0'dan büyük olmalıdır.", nameof(newQuantity));
            }
            
            Quantity = newQuantity;
            LastModifiedAt = DateTime.UtcNow;
        }
        
        public void UpdateNotes(string notes)
        {
            Notes = notes;
            LastModifiedAt = DateTime.UtcNow;
        }
        
        public void MarkAsCompleted()
        {
            IsCompleted = true;
            LastModifiedAt = DateTime.UtcNow;
        }
    }
} 