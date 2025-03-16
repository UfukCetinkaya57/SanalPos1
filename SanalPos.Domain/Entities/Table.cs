using System;
using SanalPos.Domain.Common;
using SanalPos.Domain.Enums;
using SanalPos.Domain.Exceptions;

namespace SanalPos.Domain.Entities
{
    public class Table : BaseAuditableEntity
    {
        public string Number { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Capacity { get; set; }
        public TableStatus Status { get; set; } = TableStatus.Available;
        public string Location { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public new bool IsDeleted { get; set; } = false;
        public Guid? OrderId { get; set; }
        public string? QrCode { get; private set; }
        public Guid? CurrentOrderId { get; private set; }
        
        public virtual Order? CurrentOrder { get; private set; }

        public Table(string number, int capacity, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new DomainException("Masa numarası boş olamaz.");
            
            if (capacity <= 0)
                throw new DomainException("Masa kapasitesi sıfırdan büyük olmalıdır.");
            
            Number = number;
            Capacity = capacity;
            Description = description;
            Status = TableStatus.Available;
            IsActive = true;
            CreatedBy = "System";
            LastModifiedBy = "System";
            CreatedAt = DateTime.UtcNow;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void UpdateDetails(string number, int capacity, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new DomainException("Masa numarası boş olamaz.");
            
            if (capacity <= 0)
                throw new DomainException("Masa kapasitesi sıfırdan büyük olmalıdır.");
            
            Number = number;
            Capacity = capacity;
            Description = description;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void SetStatus(TableStatus status)
        {
            if (status == TableStatus.Occupied && CurrentOrderId == null)
                throw new DomainException("Dolu olarak işaretlemek için bir sipariş ID'si gereklidir.");
            
            Status = status;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void AssignOrder(Guid orderId)
        {
            if (Status != TableStatus.Available && Status != TableStatus.Reserved)
                throw new DomainException("Sadece müsait veya rezerve edilmiş masalara sipariş atanabilir.");
            
            CurrentOrderId = orderId;
            Status = TableStatus.Occupied;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void ClearOrder()
        {
            if (Status != TableStatus.Occupied)
                throw new DomainException("Sadece dolu masalardaki siparişler temizlenebilir.");
            
            CurrentOrderId = null;
            Status = TableStatus.Available;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void Reserve(string? reservationNote = null)
        {
            if (Status != TableStatus.Available)
                throw new DomainException("Sadece müsait masalar rezerve edilebilir.");
            
            Status = TableStatus.Reserved;
            Description = string.IsNullOrEmpty(reservationNote) 
                ? Description 
                : $"{Description} - Rezervasyon: {reservationNote}";
            
            LastModifiedAt = DateTime.UtcNow;
        }

        public void GenerateQrCode(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new DomainException("QR kod oluşturmak için geçerli bir URL gereklidir.");
            
            // Gerçek bir uygulamada bu kısımda QR kod oluşturma işlemi yapılır
            QrCode = $"{baseUrl}/masalar/{Number}";
            LastModifiedAt = DateTime.UtcNow;
        }
    }
} 