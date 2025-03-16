using AutoMapper;
using MediatR;
using SanalPos.Application.DTOs;
using SanalPos.Application.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SanalPos.Application.Features.PaymentTransactions.Queries.GetPaymentTransactionsByOrderId
{
    public class GetPaymentTransactionsByOrderIdQueryHandler : IRequestHandler<GetPaymentTransactionsByOrderIdQuery, IEnumerable<PaymentTransactionDto>>
    {
        private readonly IPaymentTransactionRepository _paymentTransactionRepository;
        private readonly IMapper _mapper;

        public GetPaymentTransactionsByOrderIdQueryHandler(
            IPaymentTransactionRepository paymentTransactionRepository,
            IMapper mapper)
        {
            _paymentTransactionRepository = paymentTransactionRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PaymentTransactionDto>> Handle(GetPaymentTransactionsByOrderIdQuery request, CancellationToken cancellationToken)
        {
            var transactions = await _paymentTransactionRepository.GetByOrderIdAsync(request.OrderId);
            return _mapper.Map<IEnumerable<PaymentTransactionDto>>(transactions);
        }
    }
} 