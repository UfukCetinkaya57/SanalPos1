using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using SanalPos.Application.Common.Interfaces;
using SanalPos.Application.Interfaces;
using SanalPos.Domain.Entities;
using SanalPos.Infrastructure.Repositories;

namespace SanalPos.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _dbContext;
        private IDbContextTransaction _transaction;
        private bool _disposed;

        // Repository instances
        private IRepository<User> _users;
        private IRepository<Order> _orders;
        private IRepository<OrderItem> _orderItems;
        private IRepository<Product> _products;
        private IRepository<ProductCategory> _productCategories;
        private IRepository<Invoice> _invoices;
        private IPaymentTransactionRepository _paymentTransactions;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IRepository<User> Users => _users ??= new Repository<User>(_dbContext);
        public IRepository<Order> Orders => _orders ??= new Repository<Order>(_dbContext);
        public IRepository<OrderItem> OrderItems => _orderItems ??= new Repository<OrderItem>(_dbContext);
        public IRepository<Product> Products => _products ??= new Repository<Product>(_dbContext);
        public IRepository<ProductCategory> ProductCategories => _productCategories ??= new Repository<ProductCategory>(_dbContext);
        public IRepository<Invoice> Invoices => _invoices ??= new Repository<Invoice>(_dbContext);
        public IPaymentTransactionRepository PaymentTransactions => _paymentTransactions ??= new PaymentTransactionRepository(_dbContext);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                if (_transaction != null)
                {
                    await _transaction.RollbackAsync();
                }
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _dbContext.Dispose();
                _transaction?.Dispose();
            }
            _disposed = true;
        }
    }
} 