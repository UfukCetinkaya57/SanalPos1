using System;
using System.Collections.Generic;
using System.Linq;
using SanalPos.Domain.Common;
using SanalPos.Domain.Enums;
using SanalPos.Domain.Exceptions;

namespace SanalPos.Domain.Entities
{
    public class Order : BaseEntity
    {
        public string OrderNumber { get; set; } = null!;
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        public string? ShippingAddress { get; set; }
        public string? BillingAddress { get; set; }
        public string? Notes { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? Discount { get; set; }
        public decimal? Tax { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public Guid? CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        
        // Items için farklı bir isimle erişim kolaylığı
        public virtual ICollection<OrderItem> Items => OrderItems;

        public Order()
        {
            OrderDate = DateTime.UtcNow;
            Status = OrderStatus.Pending;
            PaymentStatus = PaymentStatus.Pending;
        }

        public void AddOrderItem(OrderItem item)
        {
            OrderItems.Add(item);
            CalculateTotalAmount();
        }

        public void RemoveOrderItem(OrderItem item)
        {
            OrderItems.Remove(item);
            CalculateTotalAmount();
        }

        private void CalculateTotalAmount()
        {
            TotalAmount = 0;
            foreach (var item in OrderItems)
            {
                TotalAmount += item.UnitPrice * item.Quantity;
            }

            if (Discount.HasValue)
            {
                TotalAmount -= Discount.Value;
            }

            if (Tax.HasValue)
            {
                TotalAmount += Tax.Value;
            }
        }

        public void UpdateStatus(OrderStatus newStatus)
        {
            Status = newStatus;
            switch (newStatus)
            {
                case OrderStatus.Shipped:
                    ShippedDate = DateTime.UtcNow;
                    break;
                case OrderStatus.Delivered:
                    DeliveredDate = DateTime.UtcNow;
                    break;
            }
        }

        public void UpdatePaymentStatus(PaymentStatus newStatus)
        {
            PaymentStatus = newStatus;
        }

        public void AddItem(Guid productId, int quantity, decimal unitPrice, string notes = null)
        {
            if (Status != OrderStatus.Pending)
            {
                throw new DomainException("Sadece yeni oluşturulmuş siparişlere ürün eklenebilir.");
            }
            
            var existingItem = OrderItems.FirstOrDefault(i => i.ProductId == productId);
            
            if (existingItem != null)
            {
                existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                var orderItem = new OrderItem(this.Id, productId, quantity, unitPrice, notes);
                OrderItems.Add(orderItem);
            }
            
            CalculateTotalAmount();
            LastModifiedAt = DateTime.UtcNow;
        }
        
        public void RemoveItem(Guid orderItemId)
        {
            if (Status != OrderStatus.Pending)
            {
                throw new DomainException("Sadece yeni oluşturulmuş siparişlerden ürün çıkarılabilir.");
            }
            
            var item = OrderItems.FirstOrDefault(i => i.Id == orderItemId);
            if (item == null)
            {
                throw new DomainException("Belirtilen sipariş kalemi bulunamadı.");
            }
            
            OrderItems.Remove(item);
            CalculateTotalAmount();
            LastModifiedAt = DateTime.UtcNow;
        }
        
        public void UpdateItemQuantity(Guid orderItemId, int newQuantity)
        {
            if (Status != OrderStatus.Pending)
            {
                throw new DomainException("Sadece yeni oluşturulmuş siparişlerdeki ürün miktarı değiştirilebilir.");
            }
            
            var item = OrderItems.FirstOrDefault(i => i.Id == orderItemId);
            if (item == null)
            {
                throw new DomainException("Belirtilen sipariş kalemi bulunamadı.");
            }
            
            if (newQuantity <= 0)
            {
                OrderItems.Remove(item);
            }
            else
            {
                item.UpdateQuantity(newQuantity);
            }
            
            CalculateTotalAmount();
            LastModifiedAt = DateTime.UtcNow;
        }
        
        public void SubmitOrder()
        {
            if (Status != OrderStatus.Pending)
            {
                throw new DomainException("Sadece yeni oluşturulmuş siparişler gönderilebilir.");
            }
            
            if (!OrderItems.Any())
            {
                throw new DomainException("Boş sipariş gönderilemez.");
            }
            
            Status = OrderStatus.Processing;
            LastModifiedAt = DateTime.UtcNow;
        }
        
        public void StartProcessing()
        {
            if (Status != OrderStatus.Processing)
            {
                throw new DomainException("Sadece gönderilmiş siparişler işleme alınabilir.");
            }
            
            Status = OrderStatus.Shipped;
            LastModifiedAt = DateTime.UtcNow;
        }
        
        public void CompleteOrder()
        {
            if (Status != OrderStatus.Shipped)
            {
                throw new DomainException("Sadece işlemdeki siparişler tamamlanabilir.");
            }
            
            Status = OrderStatus.Delivered;
            DeliveredDate = DateTime.UtcNow;
            LastModifiedAt = DateTime.UtcNow;
        }
        
        public void CancelOrder(string reason)
        {
            if (Status == OrderStatus.Delivered || Status == OrderStatus.Cancelled)
            {
                throw new DomainException("Tamamlanmış veya iptal edilmiş siparişler iptal edilemez.");
            }
            
            Status = OrderStatus.Cancelled;
            Notes = string.IsNullOrEmpty(Notes) 
                ? $"İptal Sebebi: {reason}" 
                : $"{Notes} | İptal Sebebi: {reason}";
            
            LastModifiedAt = DateTime.UtcNow;
        }
        
        public void ProcessPayment(PaymentMethod paymentMethod, string transactionId, decimal? discountAmount = null, decimal? taxAmount = null)
        {
            if (PaymentStatus != PaymentStatus.Pending)
            {
                throw new DomainException("Bu sipariş zaten işlemde.");
            }
            
            if (Status != OrderStatus.Delivered)
            {
                throw new DomainException("Sadece tamamlanmış siparişler ödenebilir.");
            }
            
            PaymentStatus = PaymentStatus.Completed;
            Discount = discountAmount;
            Tax = taxAmount;
            
            // İndirim ve vergi varsa toplam tutarı güncelle
            if (discountAmount.HasValue || taxAmount.HasValue)
            {
                CalculateTotalAmount();
            }
            
            LastModifiedAt = DateTime.UtcNow;
        }
    }
} 