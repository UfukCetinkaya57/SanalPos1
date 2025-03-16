using Microsoft.Extensions.Logging;
using SanalPos.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace SanalPos.Infrastructure.Services
{
    public class PaymentGatewayService : IPaymentGatewayService
    {
        private readonly ILogger<PaymentGatewayService> _logger;

        public PaymentGatewayService(ILogger<PaymentGatewayService> logger)
        {
            _logger = logger;
        }

        public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
        {
            _logger.LogInformation("Ödeme işlemi başlatıldı: {TransactionNumber}, Tutar: {Amount}", 
                request.TransactionNumber, request.Amount);

            // Gerçek bir ödeme geçidi entegrasyonu burada olacak
            // Şimdilik test amaçlı bir simülasyon yapıyoruz
            await Task.Delay(500); // Ödeme işlemini simüle etmek için gecikme

            // Rastgele başarılı/başarısız sonuç üret (gerçek uygulamada bu kısım ödeme geçidi API'sine göre değişecek)
            var isSuccessful = new Random().Next(0, 10) < 9; // %90 başarı oranı
            
            var result = new PaymentResult
            {
                IsSuccessful = isSuccessful,
                AuthorizationCode = isSuccessful ? GenerateRandomCode(6) : null,
                ReferenceNumber = isSuccessful ? $"REF-{DateTime.Now:yyMMdd}-{new Random().Next(1000, 9999)}" : null,
                ResponseCode = isSuccessful ? "00" : "05",
                ResponseMessage = isSuccessful ? "İşlem başarılı" : "İşlem reddedildi"
            };

            _logger.LogInformation("Ödeme işlemi tamamlandı: {TransactionNumber}, Sonuç: {IsSuccessful}", 
                request.TransactionNumber, result.IsSuccessful);

            return result;
        }

        public async Task<RefundResult> ProcessRefundAsync(RefundRequest request)
        {
            _logger.LogInformation("İade işlemi başlatıldı: {OriginalTransactionNumber}, Tutar: {RefundAmount}", 
                request.OriginalTransactionNumber, request.RefundAmount);

            // Gerçek bir ödeme geçidi entegrasyonu burada olacak
            // Şimdilik test amaçlı bir simülasyon yapıyoruz
            await Task.Delay(500); // İade işlemini simüle etmek için gecikme

            // Rastgele başarılı/başarısız sonuç üret (gerçek uygulamada bu kısım ödeme geçidi API'sine göre değişecek)
            var isSuccessful = new Random().Next(0, 10) < 9; // %90 başarı oranı
            
            var result = new RefundResult
            {
                IsSuccessful = isSuccessful,
                RefundReferenceNumber = isSuccessful ? $"REFUND-{DateTime.Now:yyMMdd}-{new Random().Next(1000, 9999)}" : null,
                ResponseCode = isSuccessful ? "00" : "05",
                ResponseMessage = isSuccessful ? "İade işlemi başarılı" : "İade işlemi reddedildi"
            };

            _logger.LogInformation("İade işlemi tamamlandı: {OriginalTransactionNumber}, Sonuç: {IsSuccessful}", 
                request.OriginalTransactionNumber, result.IsSuccessful);

            return result;
        }

        private string GenerateRandomCode(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new char[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = chars[random.Next(chars.Length)];
            }

            return new string(result);
        }
    }
} 