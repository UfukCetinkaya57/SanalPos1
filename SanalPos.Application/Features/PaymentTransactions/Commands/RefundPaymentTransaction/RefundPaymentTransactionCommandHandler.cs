using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SanalPos.Application.DTOs;
using SanalPos.Application.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SanalPos.Application.Features.PaymentTransactions.Commands.RefundPaymentTransaction
{
    public class RefundPaymentTransactionCommandHandler : IRequestHandler<RefundPaymentTransactionCommand, PaymentTransactionDto>
    {
        private readonly IPaymentTransactionRepository _paymentTransactionRepository;
        private readonly IPaymentGatewayService _paymentGatewayService;
        private readonly IMapper _mapper;
        private readonly ILogger<RefundPaymentTransactionCommandHandler> _logger;

        public RefundPaymentTransactionCommandHandler(
            IPaymentTransactionRepository paymentTransactionRepository,
            IPaymentGatewayService paymentGatewayService,
            IMapper mapper,
            ILogger<RefundPaymentTransactionCommandHandler> logger)
        {
            _paymentTransactionRepository = paymentTransactionRepository;
            _paymentGatewayService = paymentGatewayService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaymentTransactionDto> Handle(RefundPaymentTransactionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Ödeme iadesi başlatılıyor. İşlem ID: {TransactionId}, İade Tutarı: {RefundAmount}", 
                request.TransactionId, request.RefundAmount);

            // İşlem kontrolü
            var transaction = await _paymentTransactionRepository.GetByIdAsync(request.TransactionId);
            if (transaction == null)
            {
                _logger.LogError("İşlem bulunamadı. İşlem ID: {TransactionId}", request.TransactionId);
                throw new ApplicationException($"İşlem bulunamadı. ID: {request.TransactionId}");
            }

            // İşlem başarılı mı kontrolü
            if (!transaction.IsSuccessful)
            {
                _logger.LogError("Başarısız işlem için iade yapılamaz. İşlem ID: {TransactionId}", request.TransactionId);
                throw new ApplicationException("Başarısız işlem için iade yapılamaz.");
            }

            // İşlem zaten iade edilmiş mi kontrolü
            if (transaction.IsRefunded)
            {
                _logger.LogError("İşlem zaten iade edilmiş. İşlem ID: {TransactionId}", request.TransactionId);
                throw new ApplicationException("İşlem zaten iade edilmiş.");
            }

            // İade tutarı kontrolü
            if (request.RefundAmount > transaction.Amount)
            {
                _logger.LogError("İade tutarı işlem tutarından büyük olamaz. İşlem ID: {TransactionId}, İşlem Tutarı: {Amount}, İade Tutarı: {RefundAmount}", 
                    request.TransactionId, transaction.Amount, request.RefundAmount);
                throw new ApplicationException("İade tutarı işlem tutarından büyük olamaz.");
            }

            try
            {
                // İade işlemini gerçekleştir
                var refundResult = await _paymentGatewayService.ProcessRefundAsync(new RefundRequest
                {
                    OriginalTransactionNumber = transaction.TransactionNumber,
                    OriginalReferenceNumber = transaction.ReferenceNumber,
                    RefundAmount = request.RefundAmount
                });

                if (refundResult.IsSuccessful)
                {
                    // İade başarılı ise işlemi güncelle
                    transaction.ProcessRefund(request.RefundAmount, refundResult.RefundReferenceNumber, request.RefundReason);
                    await _paymentTransactionRepository.UpdateAsync(transaction);

                    _logger.LogInformation("İade işlemi başarılı. İşlem No: {TransactionNumber}, İade Referans No: {RefundReferenceNumber}", 
                        transaction.TransactionNumber, transaction.RefundReferenceNumber);
                }
                else
                {
                    _logger.LogWarning("İade işlemi başarısız. İşlem No: {TransactionNumber}, Hata Kodu: {ResponseCode}, Hata Mesajı: {ResponseMessage}", 
                        transaction.TransactionNumber, refundResult.ResponseCode, refundResult.ResponseMessage);
                    throw new ApplicationException($"İade işlemi başarısız: {refundResult.ResponseMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İade işlemi sırasında hata oluştu. İşlem No: {TransactionNumber}", transaction.TransactionNumber);
                throw;
            }

            return _mapper.Map<PaymentTransactionDto>(transaction);
        }
    }
} 