namespace SanalPos.Application.Common.Models
{
    public class PaymentResult
    {
        public bool IsSuccess { get; set; }
        public string TransactionId { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ResponseCode { get; set; }
        public string AuthCode { get; set; }
        public string HostReferenceNumber { get; set; }

        public static PaymentResult Success(string transactionId, string responseCode, string authCode, string hostReferenceNumber)
        {
            return new PaymentResult
            {
                IsSuccess = true,
                TransactionId = transactionId,
                ResponseCode = responseCode,
                AuthCode = authCode,
                HostReferenceNumber = hostReferenceNumber
            };
        }

        public static PaymentResult Failure(string errorCode, string errorMessage)
        {
            return new PaymentResult
            {
                IsSuccess = false,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage
            };
        }
    }
}