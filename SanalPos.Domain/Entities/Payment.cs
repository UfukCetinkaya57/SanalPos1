using System;
using SanalPos.Domain.Common;
using SanalPos.Domain.Enums;

namespace SanalPos.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? TransactionId { get; set; }
        public PaymentStatus Status { get; set; }
        public string? ErrorMessage { get; set; }

        public virtual Order? Order { get; set; }
    }
} 