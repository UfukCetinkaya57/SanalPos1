namespace SanalPos.Application.Common.Models
{
    public class PaymentStatusResult
    {
        public bool IsSuccess { get; set; }
        public string TransactionId { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public static PaymentStatusResult Success(string transactionId, string status, decimal amount, string currency)
        {
            return new PaymentStatusResult
            {
                IsSuccess = true,
                TransactionId = transactionId,
                Status = status,
                Amount = amount,
                Currency = currency
            };
        }

        public static PaymentStatusResult Failure(string errorCode, string errorMessage)
        {
            return new PaymentStatusResult
            {
                IsSuccess = false,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage
            };
        }
    }
} 