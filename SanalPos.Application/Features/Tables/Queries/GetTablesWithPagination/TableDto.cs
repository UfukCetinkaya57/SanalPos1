using System;
using AutoMapper;
using SanalPos.Application.Common.Mappings;
using SanalPos.Domain.Entities;
using SanalPos.Domain.Enums;

namespace SanalPos.Application.Features.Tables.Queries.GetTablesWithPagination
{
    public class TableDto : IMapFrom<Table>
    {
        public Guid Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public TableStatus Status { get; set; }
        public string Location { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public Guid? OrderId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Table, TableDto>();
        }
    }
} 