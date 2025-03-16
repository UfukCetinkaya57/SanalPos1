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
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly ApplicationDbContext _context;

        public InvoiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Invoice> GetByIdAsync(Guid id)
        {
            return await _context.Invoices
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            return await _context.Invoices
                .OrderByDescending(i => i.IssueDate)
                .ToListAsync();
        }

        public async Task<Invoice> GetByOrderIdAsync(Guid orderId)
        {
            return await _context.Invoices
                .FirstOrDefaultAsync(i => i.OrderId == orderId);
        }

        public async Task<IEnumerable<Invoice>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Invoices
                .Include(i => i.Order)
                .Where(i => i.Order.CustomerId == userId)
                .OrderByDescending(i => i.IssueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetPaidInvoicesAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Invoices
                .Where(i => i.IsPaid && i.IssueDate >= startDate && i.IssueDate <= endDate)
                .OrderByDescending(i => i.IssueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetUnpaidInvoicesAsync()
        {
            return await _context.Invoices
                .Where(i => !i.IsPaid)
                .OrderByDescending(i => i.IssueDate)
                .ToListAsync();
        }

        public async Task AddAsync(Invoice invoice)
        {
            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetTotalPaidAmountAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Invoices
                .Where(i => i.IsPaid && i.IssueDate >= startDate && i.IssueDate <= endDate)
                .SumAsync(i => i.TotalAmount);
        }

        public async Task<int> CountAsync(Func<Invoice, bool> predicate = null)
        {
            if (predicate == null)
            {
                return await _context.Invoices.CountAsync();
            }
            
            return _context.Invoices.Where(predicate).Count();
        }
    }
} 