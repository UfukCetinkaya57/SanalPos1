using System.Threading.Tasks;
using SanalPos.Application.Common.Models;

namespace SanalPos.Application.Common.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResult> ProcessPaymentAsync(PaymentRequest paymentRequest);
        Task<PaymentResult> RefundPaymentAsync(RefundRequest refundRequest);
        Task<PaymentStatusResult> CheckPaymentStatusAsync(string transactionId);
    }
} 