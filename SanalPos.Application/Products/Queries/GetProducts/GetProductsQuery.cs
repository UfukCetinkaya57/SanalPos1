using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SanalPos.Application.Common.Interfaces;

namespace SanalPos.Application.Products.Queries.GetProducts
{
    public class GetProductsQuery : IRequest<List<ProductDto>>
    {
        // Filtreleme özellikleri eklenebilir
        public bool? IsAvailable { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }

    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetProductsQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Products.AsQueryable();

            // Filtreleme
            if (request.IsAvailable.HasValue)
            {
                query = query.Where(p => p.IsAvailable == request.IsAvailable.Value);
            }

            if (request.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= request.MinPrice.Value);
            }

            if (request.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= request.MaxPrice.Value);
            }

            return await query
                .Include(p => p.Category)
                .OrderBy(p => p.Category.DisplayOrder)
                .ThenBy(p => p.Name)
                .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
    }
} 