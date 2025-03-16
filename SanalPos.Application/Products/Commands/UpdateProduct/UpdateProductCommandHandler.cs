using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SanalPos.Application.Common.Exceptions;
using SanalPos.Application.Common.Interfaces;
using SanalPos.Domain.Entities;

namespace SanalPos.Application.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateProductCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Products
                .FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Product), request.Id);
            }

            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.Price = request.Price;
            entity.StockQuantity = request.StockQuantity;
            entity.ImageUrl = request.ImageUrl;
            entity.ProductCategoryId = request.ProductCategoryId;
            entity.LastModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
} 