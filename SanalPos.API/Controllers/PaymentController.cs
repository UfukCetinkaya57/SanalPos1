using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SanalPos.Application.Common.Interfaces;
using SanalPos.Application.Common.Models;

namespace SanalPos.API.Controllers
{
    public class PaymentController : BaseApiController
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("process")]
        public async Task<ActionResult<PaymentResult>> ProcessPayment(PaymentRequest request)
        {
            var result = await _paymentService.ProcessPaymentAsync(request);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("refund")]
        [Authorize(Roles = "Administrator,Manager")]
        public async Task<ActionResult<PaymentResult>> RefundPayment(RefundRequest request)
        {
            var result = await _paymentService.RefundPaymentAsync(request);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("status/{transactionId}")]
        public async Task<ActionResult<PaymentStatusResult>> CheckPaymentStatus(string transactionId)
        {
            var result = await _paymentService.CheckPaymentStatusAsync(transactionId);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}