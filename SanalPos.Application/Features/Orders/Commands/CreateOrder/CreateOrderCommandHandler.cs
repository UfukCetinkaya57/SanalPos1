using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SanalPos.Application.Common.Interfaces;
using SanalPos.Domain.Entities;
using SanalPos.Domain.Enums;

namespace SanalPos.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<CreateOrderCommandHandler> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;

        public CreateOrderCommandHandler(
            IApplicationDbContext context,
            ILogger<CreateOrderCommandHandler> logger,
            ICurrentUserService currentUserService,
            IDateTime dateTime)
        {
            _context = context;
            _logger = logger;
            _currentUserService = currentUserService;
            _dateTime = dateTime;
        }

        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Yeni sipariş oluşturuluyor: {CustomerName}, {ItemCount} ürün", 
                request.CustomerName, request.Items.Count);

            // Yeni sipariş oluştur
            var order = new Order
            {
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                CustomerPhone = request.CustomerPhone,
                ShippingAddress = request.ShippingAddress,
                BillingAddress = request.BillingAddress,
                Notes = request.Notes,
                OrderDate = _dateTime.Now,
                Status = OrderStatus.Pending,
                PaymentStatus = PaymentStatus.Pending,
                CreatedBy = _currentUserService.UserId?.ToString() ?? "system",
                LastModifiedBy = _currentUserService.UserId?.ToString() ?? "system"
            };

            // Ürünleri ekle
            foreach (var item in request.Items)
            {
                var product = await _context.Products
                    .FindAsync(new object[] { item.ProductId }, cancellationToken);

                if (product == null)
                {
                    throw new Exception($"Ürün bulunamadı: {item.ProductId}");
                }

                // Stok kontrolü
                if (product.StockQuantity < item.Quantity)
                {
                    throw new Exception($"Yetersiz stok. Ürün: {product.Name}, Mevcut: {product.StockQuantity}, İstenen: {item.Quantity}");
                }

                // Ürünü siparişe ekle
                order.AddItem(product.Id, item.Quantity, product.Price, item.Notes);
                
                // Stok düşüşü
                product.DeductStock(item.Quantity);
            }

            // Siparişi veritabanına ekle
            _context.Orders.Add(order);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Sipariş başarıyla oluşturuldu. Sipariş ID: {OrderId}", order.Id);

            return order.Id;
        }
    }
} 