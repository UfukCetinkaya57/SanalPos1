using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SanalPos.Domain.Common;

namespace SanalPos.Application.Common.Interfaces
{
    /// <summary>
    /// Tüm repository'ler için generic base interface
    /// </summary>
    /// <typeparam name="T">Entity tipi</typeparam>
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(Guid id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }
} 