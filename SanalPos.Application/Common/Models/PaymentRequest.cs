using System;

namespace SanalPos.Application.Common.Models
{
    public class PaymentRequest
    {
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string Cvv { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "TRY";
        public Guid OrderId { get; set; }
        public string Description { get; set; }
        public int Installment { get; set; } = 0; // 0 = Tek çekim
    }
} 