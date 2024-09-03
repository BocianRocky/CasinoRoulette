using System.Security.Claims;
using CasinoRoulette.DTO;
using CasinoRoulette.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasinoRoulette.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BetController : ControllerBase
{
    
    private readonly IBetService _betService;
    

    public BetController(IBetService betService)
    {
        _betService = betService;
    }
    
         [Authorize]
         [HttpPut("results")]
         public async Task<IActionResult> AssignBetResults([FromBody] RouletteResultDto resultDto)
         {
             var playerIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

             if (playerIdClaim == null || !int.TryParse(playerIdClaim, out int playerId))
             {
                 return Unauthorized("user ID claim is missing or invalid");
             }

             var AmountWon = await _betService.AssignBetResults(resultDto, playerId);
             if(AmountWon == null)
             {
                 return BadRequest("Failed to assign bet results");
             }
     
             return Ok(AmountWon);
         }
    
    [Authorize]
    [HttpPost("bet")]
    public async Task<IActionResult> Bet([FromBody] BetDto betDto)
    {
        var playerIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (playerIdClaim == null || !int.TryParse(playerIdClaim, out int playerId))
        {
            return Unauthorized("user ID claim is missing or invalid");
        }

        var betId =await _betService.CreateBet(betDto, playerId);
        if (betId == null)
        {
            return NotFound("bet is not exists");
        }

        return Ok(new { BetId = betId });
    }

    [Authorize]
    [HttpDelete("{spinId}")]
    public async Task<IActionResult> DeleteBets(int spinId)
    {
        var playerIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (playerIdClaim == null || !int.TryParse(playerIdClaim, out int playerId))
        {
            return Unauthorized("user ID claim is missing or invalid");
        }

        await _betService.DeleteBetsFromSpinAndPlayer(spinId, playerId);
        return Ok();
    }
    
    
}