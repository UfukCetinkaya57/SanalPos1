using System;

namespace SanalPos.Application.Common.Models
{
    public class RefundRequest
    {
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "TRY";
        public Guid OrderId { get; set; }
        public string Reason { get; set; }
    }
} 