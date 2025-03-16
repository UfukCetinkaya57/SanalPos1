using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SanalPos.Application.Common.Models;
using SanalPos.Application.Features.Tables.Commands.CreateTable;
using SanalPos.Application.Features.Tables.Commands.DeleteTable;
using SanalPos.Application.Features.Tables.Commands.UpdateTable;
using SanalPos.Application.Features.Tables.Commands.UpdateTableStatus;
using SanalPos.Application.Features.Tables.Queries.GetTableDetail;
using SanalPos.Application.Features.Tables.Queries.GetTablesWithPagination;
using SanalPos.Domain.Enums;

namespace SanalPos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TablesController : ApiControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PaginatedList<TableDto>>> GetTables([FromQuery] GetTablesWithPaginationQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<TableDetailVm>> GetTable(Guid id)
        {
            return await Mediator.Send(new GetTableDetailQuery { Id = id });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateTableCommand command)
        {
            // CreatedBy değerini otomatik olarak ayarla
            var id = await Mediator.Send(command);
            
            return CreatedAtAction(nameof(GetTable), new { id }, id);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(Guid id, [FromBody] UpdateTableCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Yol parametresi, komut ID'si ile eşleşmiyor.");
            }

            await Mediator.Send(command);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await Mediator.Send(new DeleteTableCommand { Id = id });

            return NoContent();
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult> UpdateStatus(Guid id, [FromBody] UpdateTableStatusRequest request)
        {
            var command = new UpdateTableStatusCommand
            {
                Id = id,
                Status = request.Status,
                OrderId = request.OrderId
            };

            await Mediator.Send(command);

            return NoContent();
        }
    }

    public class UpdateTableStatusRequest
    {
        public TableStatus Status { get; set; }
        public Guid? OrderId { get; set; }
    }
} 