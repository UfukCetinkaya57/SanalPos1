using System;
using MediatR;
using SanalPos.Domain.Enums;

namespace SanalPos.Application.Features.Orders.Commands.UpdateOrder
{
    public class UpdateOrderCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public OrderStatus Status { get; set; }
        public string? Notes { get; set; }
    }
} 