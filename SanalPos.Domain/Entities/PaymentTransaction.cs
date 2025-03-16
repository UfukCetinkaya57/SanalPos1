using System;
using SanalPos.Domain.Common;
using SanalPos.Domain.Enums;

namespace SanalPos.Domain.Entities
{
    public class PaymentTransaction : BaseEntity
    {
        public string TransactionNumber { get; private set; }
        public Guid OrderId { get; private set; }
        public Order Order { get; private set; }
        public decimal Amount { get; private set; }
        public PaymentMethod PaymentMethod { get; private set; }
        public string CardNumber { get; private set; }
        public string CardHolderName { get; private set; }
        public string AuthorizationCode { get; private set; }
        public string ReferenceNumber { get; private set; }
        public bool IsSuccessful { get; private set; }
        public string ResponseCode { get; private set; }
        public string ResponseMessage { get; private set; }
        public DateTime TransactionDate { get; private set; }
        public bool IsRefunded { get; private set; }
        public DateTime? RefundDate { get; private set; }
        public string RefundReferenceNumber { get; private set; }

        private PaymentTransaction() : base()
        {
            // EF Core için gerekli
        }

        public PaymentTransaction(
            Guid orderId, 
            decimal amount, 
            PaymentMethod paymentMethod,
            string cardNumber = null,
            string cardHolderName = null,
            string authorizationCode = null,
            string referenceNumber = null,
            bool isSuccessful = false,
            string responseCode = null,
            string responseMessage = null) : base()
        {
            OrderId = orderId;
            Amount = amount;
            PaymentMethod = paymentMethod;
            CardNumber = cardNumber;
            CardHolderName = cardHolderName;
            AuthorizationCode = authorizationCode;
            ReferenceNumber = referenceNumber;
            IsSuccessful = isSuccessful;
            ResponseCode = responseCode;
            ResponseMessage = responseMessage;
            TransactionDate = DateTime.UtcNow;
            IsRefunded = false;
            TransactionNumber = GenerateTransactionNumber();
        }

        public void MarkAsSuccessful(string authorizationCode, string referenceNumber, string responseCode = "00", string responseMessage = "Success")
        {
            IsSuccessful = true;
            AuthorizationCode = authorizationCode;
            ReferenceNumber = referenceNumber;
            ResponseCode = responseCode;
            ResponseMessage = responseMessage;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void MarkAsFailed(string responseCode, string responseMessage)
        {
            IsSuccessful = false;
            ResponseCode = responseCode;
            ResponseMessage = responseMessage;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void Refund(string refundReferenceNumber)
        {
            if (!IsSuccessful)
            {
                throw new InvalidOperationException("Başarısız işlemler iade edilemez.");
            }

            if (IsRefunded)
            {
                throw new InvalidOperationException("Bu işlem zaten iade edilmiş.");
            }

            IsRefunded = true;
            RefundDate = DateTime.UtcNow;
            RefundReferenceNumber = refundReferenceNumber;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void ProcessRefund(decimal refundAmount, string refundReferenceNumber, string refundReason = null)
        {
            if (!IsSuccessful)
            {
                throw new InvalidOperationException("Başarısız işlemler iade edilemez.");
            }

            if (IsRefunded)
            {
                throw new InvalidOperationException("Bu işlem zaten iade edilmiş.");
            }

            if (refundAmount > Amount)
            {
                throw new InvalidOperationException("İade tutarı işlem tutarından büyük olamaz.");
            }

            IsRefunded = true;
            RefundDate = DateTime.UtcNow;
            RefundReferenceNumber = refundReferenceNumber;
            LastModifiedAt = DateTime.UtcNow;
        }

        private string GenerateTransactionNumber()
        {
            // TRX-YılAyGün-rastgele 6 haneli sayı
            return $"TRX-{DateTime.UtcNow:yyMMdd}-{new Random().Next(100000, 999999)}";
        }
    }
} 