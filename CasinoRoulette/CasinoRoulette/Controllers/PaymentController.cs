using System.Security.Claims;
using CasinoRoulette.DTO;
using CasinoRoulette.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasinoRoulette.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }
    
    [Authorize]
    [HttpPost("payment")]
    public async Task<IActionResult> Payment([FromBody] PaymentDto payment)
    {
        if (payment == null)
        {
            return BadRequest("Payment data is missing");
        }

        var playerIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (payment.Amount < 0)
        {
            return BadRequest();
        }

        if (playerIdClaim == null || !int.TryParse(playerIdClaim, out int playerId))
        {
            return Unauthorized("User ID claim is missing or invalid.");
        }

        var result = await _paymentService.ProcessPayment(payment, playerId);
        if (!result)
        {
            return StatusCode(500, "error");
        }

        return Ok("Payment processed successfully.");
        
    }
}