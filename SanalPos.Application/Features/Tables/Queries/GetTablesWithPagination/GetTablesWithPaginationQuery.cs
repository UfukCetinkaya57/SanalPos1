using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SanalPos.Application.Common.Interfaces;
using SanalPos.Application.Common.Mappings;
using SanalPos.Application.Common.Models;
using SanalPos.Domain.Enums;

namespace SanalPos.Application.Features.Tables.Queries.GetTablesWithPagination
{
    public class GetTablesWithPaginationQuery : IRequest<PaginatedList<TableDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Location { get; set; }
        public TableStatus? Status { get; set; }
        public bool? IsActive { get; set; }
    }

    public class GetTablesWithPaginationQueryHandler : IRequestHandler<GetTablesWithPaginationQuery, PaginatedList<TableDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetTablesWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PaginatedList<TableDto>> Handle(GetTablesWithPaginationQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Tables.AsQueryable();

            // Silinmiş masaları çıkar
            query = query.Where(t => !t.IsDeleted);

            // Duruma göre filtrele
            if (request.Status.HasValue)
            {
                query = query.Where(t => t.Status == request.Status.Value);
            }

            // Aktiflik durumuna göre filtrele
            if (request.IsActive.HasValue)
            {
                query = query.Where(t => t.IsActive == request.IsActive.Value);
            }

            if (!string.IsNullOrEmpty(request.Location))
            {
                query = query.Where(t => t.Location.Contains(request.Location));
            }

            // Masaları masa numarasına göre sırala
            query = query.OrderBy(t => t.Number);

            var result = await query
                .ProjectTo<TableDto>(_mapper.ConfigurationProvider)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);

            return result;
        }
    }
} 