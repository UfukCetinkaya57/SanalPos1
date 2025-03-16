using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SanalPos.Application.DTOs;
using SanalPos.Application.Interfaces;
using SanalPos.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SanalPos.Application.Features.PaymentTransactions.Commands.CreatePaymentTransaction
{
    public class CreatePaymentTransactionCommandHandler : IRequestHandler<CreatePaymentTransactionCommand, PaymentTransactionDto>
    {
        private readonly IPaymentTransactionRepository _paymentTransactionRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentGatewayService _paymentGatewayService;
        private readonly IMapper _mapper;
        private readonly ILogger<CreatePaymentTransactionCommandHandler> _logger;

        public CreatePaymentTransactionCommandHandler(
            IPaymentTransactionRepository paymentTransactionRepository,
            IOrderRepository orderRepository,
            IPaymentGatewayService paymentGatewayService,
            IMapper mapper,
            ILogger<CreatePaymentTransactionCommandHandler> logger)
        {
            _paymentTransactionRepository = paymentTransactionRepository;
            _orderRepository = orderRepository;
            _paymentGatewayService = paymentGatewayService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaymentTransactionDto> Handle(CreatePaymentTransactionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Ödeme işlemi başlatılıyor. Sipariş ID: {OrderId}, Tutar: {Amount}", request.OrderId, request.Amount);

            // Sipariş kontrolü
            var order = await _orderRepository.GetByIdAsync(request.OrderId);
            if (order == null)
            {
                _logger.LogError("Sipariş bulunamadı. Sipariş ID: {OrderId}", request.OrderId);
                throw new ApplicationException($"Sipariş bulunamadı. ID: {request.OrderId}");
            }

            // Ödeme işlemi oluşturma
            var transaction = new PaymentTransaction(
                request.OrderId,
                request.Amount,
                request.PaymentMethod,
                MaskCardNumber(request.CardNumber),
                request.CardHolderName);

            await _paymentTransactionRepository.AddAsync(transaction);

            try
            {
                // Ödeme işlemini gerçekleştir
                var paymentResult = await _paymentGatewayService.ProcessPaymentAsync(new PaymentRequest
                {
                    TransactionNumber = transaction.TransactionNumber,
                    Amount = request.Amount,
                    CardNumber = request.CardNumber,
                    CardHolderName = request.CardHolderName,
                    CardExpiryMonth = request.CardExpiryMonth,
                    CardExpiryYear = request.CardExpiryYear,
                    CardCvv = request.CardCvv
                });

                if (paymentResult.IsSuccessful)
                {
                    transaction.MarkAsSuccessful(
                        paymentResult.AuthorizationCode,
                        paymentResult.ReferenceNumber,
                        paymentResult.ResponseCode,
                        paymentResult.ResponseMessage);
                    
                    _logger.LogInformation("Ödeme başarılı. İşlem No: {TransactionNumber}, Referans No: {ReferenceNumber}", 
                        transaction.TransactionNumber, transaction.ReferenceNumber);
                }
                else
                {
                    transaction.MarkAsFailed(
                        paymentResult.ResponseCode,
                        paymentResult.ResponseMessage);
                    
                    _logger.LogWarning("Ödeme başarısız. İşlem No: {TransactionNumber}, Hata Kodu: {ResponseCode}, Hata Mesajı: {ResponseMessage}", 
                        transaction.TransactionNumber, transaction.ResponseCode, transaction.ResponseMessage);
                }

                await _paymentTransactionRepository.UpdateAsync(transaction);
            }
            catch (Exception ex)
            {
                transaction.MarkAsFailed(
                    "500",
                    $"İşlem sırasında hata oluştu: {ex.Message}");
                
                await _paymentTransactionRepository.UpdateAsync(transaction);

                _logger.LogError(ex, "Ödeme işlemi sırasında hata oluştu. İşlem No: {TransactionNumber}", transaction.TransactionNumber);
                throw;
            }

            return _mapper.Map<PaymentTransactionDto>(transaction);
        }

        private string MaskCardNumber(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length < 13)
                return cardNumber;

            return cardNumber.Substring(0, 6) + new string('*', cardNumber.Length - 10) + cardNumber.Substring(cardNumber.Length - 4);
        }
    }
} 