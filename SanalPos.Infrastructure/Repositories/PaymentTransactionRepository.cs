using Microsoft.EntityFrameworkCore;
using SanalPos.Application.Interfaces;
using SanalPos.Domain.Entities;
using SanalPos.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SanalPos.Infrastructure.Repositories
{
    public class PaymentTransactionRepository : IPaymentTransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentTransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentTransaction> GetByIdAsync(Guid id)
        {
            return await _context.PaymentTransactions
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<PaymentTransaction> GetByTransactionNumberAsync(string transactionNumber)
        {
            return await _context.PaymentTransactions
                .FirstOrDefaultAsync(t => t.TransactionNumber == transactionNumber);
        }

        public async Task<IEnumerable<PaymentTransaction>> GetByOrderIdAsync(Guid orderId)
        {
            return await _context.PaymentTransactions
                .Where(t => t.OrderId == orderId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PaymentTransaction>> GetSuccessfulTransactionsAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.PaymentTransactions
                .Where(t => t.IsSuccessful && t.CreatedAt >= startDate && t.CreatedAt <= endDate)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PaymentTransaction>> GetFailedTransactionsAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.PaymentTransactions
                .Where(t => !t.IsSuccessful && t.CreatedAt >= startDate && t.CreatedAt <= endDate)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PaymentTransaction>> GetRefundedTransactionsAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.PaymentTransactions
                .Where(t => t.IsRefunded && t.RefundDate >= startDate && t.RefundDate <= endDate)
                .OrderByDescending(t => t.RefundDate)
                .ToListAsync();
        }

        public async Task AddAsync(PaymentTransaction transaction)
        {
            await _context.PaymentTransactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PaymentTransaction transaction)
        {
            _context.PaymentTransactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetTotalTransactionAmountAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.PaymentTransactions
                .Where(t => t.IsSuccessful && !t.IsRefunded && t.CreatedAt >= startDate && t.CreatedAt <= endDate)
                .SumAsync(t => t.Amount);
        }

        public async Task<int> CountAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.PaymentTransactions
                .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate)
                .CountAsync();
        }
    }
} 