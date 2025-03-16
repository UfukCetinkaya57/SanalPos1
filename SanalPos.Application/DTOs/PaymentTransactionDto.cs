using System;

namespace SanalPos.Application.DTOs
{
    public class PaymentTransactionDto
    {
        public Guid Id { get; set; }
        public string TransactionNumber { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public string AuthorizationCode { get; set; }
        public string ReferenceNumber { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public bool IsSuccessful { get; set; }
        public bool IsRefunded { get; set; }
        public string RefundReferenceNumber { get; set; }
        public decimal? RefundAmount { get; set; }
        public DateTime? RefundDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
    }
} 