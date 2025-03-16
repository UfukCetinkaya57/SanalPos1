using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SanalPos.Application.Features.Products.Commands.CreateProduct;
using SanalPos.Application.Features.Products.Commands.UpdateProduct;
using System.Threading.Tasks;
using MediatR;
using System;

namespace SanalPos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Tüm endpoint'ler için authentication gerekli
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [AllowAnonymous] // Ürün listesi herkese açık
        public async Task<IActionResult> GetProducts()
        {
            // Örnek implementasyon
            return Ok("Ürün listesi burada gelecek.");
        }

        [HttpGet("{id}")]
        [AllowAnonymous] // Ürün detayı herkese açık
        public async Task<IActionResult> GetProduct(int id)
        {
            // Örnek implementasyon
            return Ok($"Ürün detayı id: {id}");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // Sadece Admin rolüne sahip kullanıcılar ürün ekleyebilir
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Sadece Admin rolüne sahip kullanıcılar ürün güncelleyebilir
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductCommand command)
        {
            if (id != command.Id)
                return BadRequest("Yol parametresi, komut ID'si ile eşleşmiyor.");

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Sadece Admin rolüne sahip kullanıcılar ürün silebilir
        public async Task<IActionResult> DeleteProduct(int id)
        {
            // Örnek implementasyon
            return NoContent();
        }
    }
} 