using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SanalPos.Application.Common.Interfaces;
using SanalPos.Application.Common.Models;

namespace SanalPos.Infrastructure.Payment
{
    public class MockPaymentService : IPaymentService
    {
        private readonly ILogger<MockPaymentService> _logger;
        private readonly IDateTime _dateTime;

        public MockPaymentService(ILogger<MockPaymentService> logger, IDateTime dateTime)
        {
            _logger = logger;
            _dateTime = dateTime;
        }

        public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest paymentRequest)
        {
            _logger.LogInformation("Ödeme işlemi başlatıldı: {OrderId}, {Amount} {Currency}", 
                paymentRequest.OrderId, paymentRequest.Amount, paymentRequest.Currency);

            await Task.Delay(1000); // Gerçek bir ödeme işlemini simüle etmek için

            // Test kartı kontrolü
            if (paymentRequest.CardNumber == "4111111111111111")
            {
                var transactionId = Guid.NewGuid().ToString("N");
                _logger.LogInformation("Ödeme başarılı: {TransactionId}", transactionId);
                
                return PaymentResult.Success(
                    transactionId,
                    "00", // Başarılı işlem kodu
                    "123456", // Yetkilendirme kodu
                    DateTime.Now.Ticks.ToString() // Host referans numarası
                );
            }
            else if (paymentRequest.CardNumber == "4000000000000002")
            {
                _logger.LogWarning("Yetersiz bakiye: {CardNumber}", MaskCardNumber(paymentRequest.CardNumber));
                return PaymentResult.Failure("51", "Yetersiz bakiye");
            }
            else if (paymentRequest.CardNumber == "4000000000000009")
            {
                _logger.LogWarning("Geçersiz kart: {CardNumber}", MaskCardNumber(paymentRequest.CardNumber));
                return PaymentResult.Failure("14", "Geçersiz kart numarası");
            }
            else
            {
                // Rastgele başarılı veya başarısız sonuç
                var random = new Random();
                if (random.Next(0, 10) < 8) // %80 başarı oranı
                {
                    var transactionId = Guid.NewGuid().ToString("N");
                    _logger.LogInformation("Ödeme başarılı: {TransactionId}", transactionId);
                    
                    return PaymentResult.Success(
                        transactionId,
                        "00",
                        random.Next(100000, 999999).ToString(),
                        DateTime.Now.Ticks.ToString()
                    );
                }
                else
                {
                    _logger.LogWarning("Ödeme reddedildi: {CardNumber}", MaskCardNumber(paymentRequest.CardNumber));
                    return PaymentResult.Failure("05", "Ödeme reddedildi");
                }
            }
        }

        public async Task<PaymentResult> RefundPaymentAsync(RefundRequest refundRequest)
        {
            _logger.LogInformation("İade işlemi başlatıldı: {OrderId}, {TransactionId}, {Amount} {Currency}", 
                refundRequest.OrderId, refundRequest.TransactionId, refundRequest.Amount, refundRequest.Currency);

            await Task.Delay(800); // Gerçek bir iade işlemini simüle etmek için

            // Rastgele başarılı veya başarısız sonuç
            var random = new Random();
            if (random.Next(0, 10) < 9) // %90 başarı oranı
            {
                var newTransactionId = Guid.NewGuid().ToString("N");
                _logger.LogInformation("İade başarılı: {NewTransactionId}", newTransactionId);
                
                return PaymentResult.Success(
                    newTransactionId,
                    "00",
                    random.Next(100000, 999999).ToString(),
                    DateTime.Now.Ticks.ToString()
                );
            }
            else
            {
                _logger.LogWarning("İade işlemi başarısız: {TransactionId}", refundRequest.TransactionId);
                return PaymentResult.Failure("57", "İade işlemi gerçekleştirilemedi");
            }
        }

        public async Task<PaymentStatusResult> CheckPaymentStatusAsync(string transactionId)
        {
            _logger.LogInformation("Ödeme durumu sorgulanıyor: {TransactionId}", transactionId);

            await Task.Delay(500); // Gerçek bir sorgu işlemini simüle etmek için

            // Rastgele başarılı veya başarısız sonuç
            var random = new Random();
            if (random.Next(0, 10) < 9) // %90 başarı oranı
            {
                var statuses = new[] { "APPROVED", "SETTLED", "PENDING" };
                var status = statuses[random.Next(0, statuses.Length)];
                var amount = random.Next(10, 1000) + random.NextDouble();
                
                _logger.LogInformation("Ödeme durumu: {Status}, {TransactionId}", status, transactionId);
                
                return PaymentStatusResult.Success(
                    transactionId,
                    status,
                    (decimal)Math.Round(amount, 2),
                    "TRY"
                );
            }
            else
            {
                _logger.LogWarning("Ödeme durumu sorgulanamadı: {TransactionId}", transactionId);
                return PaymentStatusResult.Failure("25", "İşlem bulunamadı");
            }
        }

        private string MaskCardNumber(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length < 13)
                return "****";

            return cardNumber.Substring(0, 6) + "******" + cardNumber.Substring(cardNumber.Length - 4);
        }
    }
} 