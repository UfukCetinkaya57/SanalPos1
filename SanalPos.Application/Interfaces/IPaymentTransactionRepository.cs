using SanalPos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SanalPos.Application.Interfaces
{
    public interface IPaymentTransactionRepository
    {
        Task<PaymentTransaction> GetByIdAsync(Guid id);
        Task<PaymentTransaction> GetByTransactionNumberAsync(string transactionNumber);
        Task<IEnumerable<PaymentTransaction>> GetByOrderIdAsync(Guid orderId);
        Task<IEnumerable<PaymentTransaction>> GetSuccessfulTransactionsAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<PaymentTransaction>> GetFailedTransactionsAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<PaymentTransaction>> GetRefundedTransactionsAsync(DateTime startDate, DateTime endDate);
        Task AddAsync(PaymentTransaction transaction);
        Task UpdateAsync(PaymentTransaction transaction);
        Task<decimal> GetTotalTransactionAmountAsync(DateTime startDate, DateTime endDate);
        Task<int> CountAsync(DateTime startDate, DateTime endDate);
    }
} 