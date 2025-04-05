using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SanalPos.Application.Products.Commands.CreateProduct;
using SanalPos.Application.Products.Commands.DeleteProduct;
using SanalPos.Application.Products.Commands.UpdateProduct;
using SanalPos.Application.Products.Queries.GetProductById;
using SanalPos.Application.Products.Queries.GetProducts;

namespace SanalPos.API.Controllers
{
    public class ProductsController : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<List<ProductDto>>> GetProducts()
        {
            return await Mediator.Send(new GetProductsQuery());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
        {
            return await Mediator.Send(new GetProductByIdQuery { Id = id });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // Sadece Admin rolüne sahip kullanýcýlar ürün ekleyebilir
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        //[HttpPost]
        //[Authorize(Roles = "Administrator,Manager")]
        //public async Task<ActionResult<Guid>> Create(CreateProductCommand command)
        //{
        //    var productId = await Mediator.Send(command);
        //    return CreatedAtAction(nameof(GetProduct), new { id = productId }, productId);
        //}

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> Update(Guid id, UpdateProductCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            await Mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await Mediator.Send(new DeleteProductCommand { Id = id });
            return NoContent();
        }
    }
} 