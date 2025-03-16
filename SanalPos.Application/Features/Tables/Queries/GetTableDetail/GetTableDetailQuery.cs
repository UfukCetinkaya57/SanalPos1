using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SanalPos.Application.Common.Interfaces;

namespace SanalPos.Application.Features.Tables.Queries.GetTableDetail
{
    public class GetTableDetailQuery : IRequest<TableDetailVm>
    {
        public Guid Id { get; set; }
    }

    public class GetTableDetailQueryHandler : IRequestHandler<GetTableDetailQuery, TableDetailVm>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetTableDetailQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TableDetailVm> Handle(GetTableDetailQuery request, CancellationToken cancellationToken)
        {
            var table = await _context.Tables
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (table == null)
            {
                throw new Exception($"Masa bulunamadı: {request.Id}");
            }

            return _mapper.Map<TableDetailVm>(table);
        }
    }
} 