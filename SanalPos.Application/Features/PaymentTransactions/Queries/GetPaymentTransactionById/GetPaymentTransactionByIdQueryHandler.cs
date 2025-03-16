using AutoMapper;
using MediatR;
using SanalPos.Application.DTOs;
using SanalPos.Application.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SanalPos.Application.Features.PaymentTransactions.Queries.GetPaymentTransactionById
{
    public class GetPaymentTransactionByIdQueryHandler : IRequestHandler<GetPaymentTransactionByIdQuery, PaymentTransactionDto>
    {
        private readonly IPaymentTransactionRepository _paymentTransactionRepository;
        private readonly IMapper _mapper;

        public GetPaymentTransactionByIdQueryHandler(
            IPaymentTransactionRepository paymentTransactionRepository,
            IMapper mapper)
        {
            _paymentTransactionRepository = paymentTransactionRepository;
            _mapper = mapper;
        }

        public async Task<PaymentTransactionDto> Handle(GetPaymentTransactionByIdQuery request, CancellationToken cancellationToken)
        {
            var transaction = await _paymentTransactionRepository.GetByIdAsync(request.Id);
            
            if (transaction == null)
            {
                throw new ApplicationException($"İşlem bulunamadı. ID: {request.Id}");
            }

            return _mapper.Map<PaymentTransactionDto>(transaction);
        }
    }
} 