using SanalPos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SanalPos.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> GetByIdAsync(Guid id);
        Task<IEnumerable<Order>> GetAllAsync();
        Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task<int> CountAsync(Func<Order, bool> predicate = null);
    }
} 