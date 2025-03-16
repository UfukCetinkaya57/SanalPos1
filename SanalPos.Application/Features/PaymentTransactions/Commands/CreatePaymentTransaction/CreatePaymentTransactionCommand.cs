using MediatR;
using SanalPos.Application.DTOs;
using SanalPos.Domain.Enums;
using System;

namespace SanalPos.Application.Features.PaymentTransactions.Commands.CreatePaymentTransaction
{
    public class CreatePaymentTransactionCommand : IRequest<PaymentTransactionDto>
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public string CardExpiryMonth { get; set; }
        public string CardExpiryYear { get; set; }
        public string CardCvv { get; set; }
    }
} 