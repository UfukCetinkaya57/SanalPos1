using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SanalPos.Application.Common.Exceptions;
using SanalPos.Application.Common.Interfaces;
using SanalPos.Domain.Entities;

namespace SanalPos.Application.Products.Commands.DeleteProduct
{
    public class DeleteProductCommand : IRequest
    {
        public Guid Id { get; set; }
    }

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(request.Id);

            if (product == null)
            {
                throw new NotFoundException(nameof(Product), request.Id);
            }

            // Sipariş kalemlerinde bu ürün kullanılıyor mu kontrol et
            var isUsedInOrderItems = await _unitOfWork.OrderItems.ExistsAsync(oi => oi.ProductId == request.Id);
            
            if (isUsedInOrderItems)
            {
                // Ürün siparişlerde kullanılıyorsa silme, sadece pasif yap
                product.SetAvailability(false);
                await _unitOfWork.Products.UpdateAsync(product);
            }
            else
            {
                // Ürün hiçbir siparişte kullanılmıyorsa tamamen sil
                await _unitOfWork.Products.DeleteAsync(product);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
} 