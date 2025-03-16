using System;
using AutoMapper;
using SanalPos.Application.Common.Mappings;
using SanalPos.Domain.Entities;
using SanalPos.Domain.Enums;

namespace SanalPos.Application.Features.Tables.Queries.GetTableDetail
{
    public class TableDetailVm : IMapFrom<Table>
    {
        public Guid Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Capacity { get; set; }
        public TableStatus Status { get; set; }
        public string Location { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public Guid? OrderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? LastModifiedAt { get; set; }
        public string? LastModifiedBy { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Table, TableDetailVm>();
        }
    }

    public class OrderDto : IMapFrom<Order>
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = null!;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Order, OrderDto>()
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));
        }
    }
} 