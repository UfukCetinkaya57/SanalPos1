using System;
using SanalPos.Domain.Enums;

namespace SanalPos.Application.Orders.Commands
{
    public class UpdateOrderCommand
    {
        public Guid Id { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string Notes { get; set; }
    }
} 