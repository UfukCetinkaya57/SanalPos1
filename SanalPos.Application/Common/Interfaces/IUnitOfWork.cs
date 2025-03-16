using System.Threading;
using System.Threading.Tasks;
using SanalPos.Domain.Entities;
using SanalPos.Application.Interfaces;

namespace SanalPos.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<User> Users { get; }
        IRepository<Order> Orders { get; }
        IRepository<OrderItem> OrderItems { get; }
        IRepository<Product> Products { get; }
        IRepository<ProductCategory> ProductCategories { get; }
        IRepository<Invoice> Invoices { get; }
        IPaymentTransactionRepository PaymentTransactions { get; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
} 