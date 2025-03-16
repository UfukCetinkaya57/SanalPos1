using MediatR;
using SanalPos.Application.DTOs;
using System;
using System.Collections.Generic;

namespace SanalPos.Application.Features.PaymentTransactions.Queries.GetPaymentTransactionsByOrderId
{
    public class GetPaymentTransactionsByOrderIdQuery : IRequest<IEnumerable<PaymentTransactionDto>>
    {
        public Guid OrderId { get; set; }
    }
} 