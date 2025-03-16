using MediatR;
using SanalPos.Application.DTOs;
using System;

namespace SanalPos.Application.Features.PaymentTransactions.Queries.GetPaymentTransactionById
{
    public class GetPaymentTransactionByIdQuery : IRequest<PaymentTransactionDto>
    {
        public Guid Id { get; set; }
    }
} 