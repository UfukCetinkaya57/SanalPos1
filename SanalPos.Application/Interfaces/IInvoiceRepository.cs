using SanalPos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SanalPos.Application.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<Invoice> GetByIdAsync(Guid id);
        Task<IEnumerable<Invoice>> GetAllAsync();
        Task<Invoice> GetByOrderIdAsync(Guid orderId);
        Task<IEnumerable<Invoice>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Invoice>> GetPaidInvoicesAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Invoice>> GetUnpaidInvoicesAsync();
        Task AddAsync(Invoice invoice);
        Task UpdateAsync(Invoice invoice);
        Task<decimal> GetTotalPaidAmountAsync(DateTime startDate, DateTime endDate);
        Task<int> CountAsync(Func<Invoice, bool> predicate = null);
    }
} 