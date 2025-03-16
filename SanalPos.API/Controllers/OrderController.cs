using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SanalPos.Application.Features.Orders.Commands.CreateOrder;
using SanalPos.Application.Features.Orders.Commands.UpdateOrder;
using System.Threading.Tasks;
using MediatR;

namespace SanalPos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Tüm endpoint'ler için authentication gerekli
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")] // Admin ve User rolüne sahip kullanıcılar erişebilir
        public async Task<IActionResult> GetOrders()
        {
            // Örnek implementasyon
            return Ok("Sipariş listesi burada gelecek.");
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetOrder(int id)
        {
            // Örnek implementasyon
            return Ok($"Sipariş detayı id: {id}");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // Sadece Admin rolüne sahip kullanıcılar sipariş oluşturabilir
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Sadece Admin rolüne sahip kullanıcılar sipariş güncelleyebilir
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderCommand command)
        {
            if (id.ToString() != command.Id.ToString())
                return BadRequest("Yol parametresi, komut ID'si ile eşleşmiyor.");

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Sadece Admin rolüne sahip kullanıcılar sipariş silebilir
        public async Task<IActionResult> DeleteOrder(int id)
        {
            // Örnek implementasyon
            return NoContent();
        }
    }
} 