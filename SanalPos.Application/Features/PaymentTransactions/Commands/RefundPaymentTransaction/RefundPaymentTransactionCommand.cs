using MediatR;
using SanalPos.Application.DTOs;
using System;

namespace SanalPos.Application.Features.PaymentTransactions.Commands.RefundPaymentTransaction
{
    public class RefundPaymentTransactionCommand : IRequest<PaymentTransactionDto>
    {
        public Guid TransactionId { get; set; }
        public decimal RefundAmount { get; set; }
        public string RefundReason { get; set; }
    }
} 