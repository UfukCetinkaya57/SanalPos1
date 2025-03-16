using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SanalPos.Domain.Entities;

namespace SanalPos.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Product> Products { get; set; }
        DbSet<ProductCategory> ProductCategories { get; set; }
        DbSet<Order> Orders { get; set; }
        DbSet<OrderItem> OrderItems { get; set; }
        DbSet<Payment> Payments { get; set; }
        DbSet<PaymentProvider> PaymentProviders { get; set; }
        DbSet<Invoice> Invoices { get; set; }
        DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        DbSet<Table> Tables { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
} 