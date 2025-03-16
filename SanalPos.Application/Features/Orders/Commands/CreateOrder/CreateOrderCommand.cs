using System;
using System.Collections.Generic;
using MediatR;

namespace SanalPos.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommand : IRequest<Guid>
    {
        public string CustomerName { get; set; } = null!;
        public string CustomerEmail { get; set; } = null!;
        public string? CustomerPhone { get; set; }
        public string ShippingAddress { get; set; } = null!;
        public string BillingAddress { get; set; } = null!;
        public string? Notes { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public string? Notes { get; set; }
    }
} 