using System.Threading.Tasks;

namespace SanalPos.Application.Interfaces
{
    public interface IPaymentGatewayService
    {
        Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request);
        Task<RefundResult> ProcessRefundAsync(RefundRequest request);
    }

    public class PaymentRequest
    {
        public string TransactionNumber { get; set; }
        public decimal Amount { get; set; }
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public string CardExpiryMonth { get; set; }
        public string CardExpiryYear { get; set; }
        public string CardCvv { get; set; }
    }

    public class PaymentResult
    {
        public bool IsSuccessful { get; set; }
        public string AuthorizationCode { get; set; }
        public string ReferenceNumber { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
    }

    public class RefundRequest
    {
        public string OriginalTransactionNumber { get; set; }
        public string OriginalReferenceNumber { get; set; }
        public decimal RefundAmount { get; set; }
    }

    public class RefundResult
    {
        public bool IsSuccessful { get; set; }
        public string RefundReferenceNumber { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
    }
} 