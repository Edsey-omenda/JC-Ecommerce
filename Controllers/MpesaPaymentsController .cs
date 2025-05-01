using JC_Ecommerce.Models.DTOs;
using JC_Ecommerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JC_Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MpesaPaymentsController : ControllerBase
    {
        private readonly IMpesaService mpesaService;

        public MpesaPaymentsController(IMpesaService mpesaService)
        {
            this.mpesaService = mpesaService;
        }

        [HttpPost("initiate")]
        [Authorize]  // Optional: Only if you want it secured
        public async Task<IActionResult> InitiatePayment([FromBody] StkPushRequestDto requestDto)
        {
            var result = await mpesaService.InitiateStkPushAsync(requestDto);

            if (result == null)
            {
                return BadRequest("Failed to initiate payment.");
            }

            return Ok(result);
        }


        [HttpGet("status/{checkoutRequestId}")]
        [Authorize]
        public async Task<IActionResult> GetPaymentStatus([FromRoute] string checkoutRequestId)
        {
            var payment = await mpesaService.GetPaymentByCheckoutIdAsync(checkoutRequestId);

            if (payment == null)
            {
                return NotFound($"No payment found with CheckoutRequestId {checkoutRequestId}");
            }

            return Ok(payment);
        }


    }
}
