using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SanalPos.Application.Common.Interfaces;
using SanalPos.Domain.Common;
using SanalPos.Domain.Entities;

namespace SanalPos.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        private readonly IModelCustomizer _modelCustomizer;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IModelCustomizer modelCustomizer = null) : base(options)
        {
            _modelCustomizer = modelCustomizer;
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ProductCategory> ProductCategories { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<Domain.Entities.Payment> Payments { get; set; } = null!;
        public DbSet<PaymentProvider> PaymentProviders { get; set; } = null!;
        public DbSet<Invoice> Invoices { get; set; } = null!;
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; } = null!;
        public DbSet<Table> Tables { get; set; } = null!;

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.CreatedBy = "System";
                        entry.Entity.LastModifiedAt = DateTime.UtcNow;
                        entry.Entity.LastModifiedBy = "System";
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedAt = DateTime.UtcNow;
                        entry.Entity.LastModifiedBy = "System";
                        break;
                }
            }

            try
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                // Log hataları
                var errorMessage = $"Veritabanı güncelleme hatası: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $" İç hata: {ex.InnerException.Message}";
                }
                
                // Hata fırlatmadan önce hata bilgilerini logla
                Console.WriteLine(errorMessage);
                throw; // Orijinal hatayı yeniden fırlat
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            _modelCustomizer?.Customize(modelBuilder);
            
            // Tables tablosunu özel olarak tanımla
            modelBuilder.Entity<Table>(entity =>
            {
                entity.ToTable("Tables");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Number)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.Property(e => e.Description)
                    .HasMaxLength(200);
                
                entity.Property(e => e.Capacity)
                    .IsRequired();
                
                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasConversion<string>();
                
                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);
                
                entity.Property(e => e.IsDeleted)
                    .IsRequired()
                    .HasDefaultValue(false);
                
                entity.Property(e => e.CreatedBy)
                    .IsRequired(false)
                    .HasMaxLength(100);
                
                entity.Property(e => e.OrderId)
                    .IsRequired(false);
            });
            
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
} 