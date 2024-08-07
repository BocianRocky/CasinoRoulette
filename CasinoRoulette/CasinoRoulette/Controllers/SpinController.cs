using CasinoRoulette.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasinoRoulette.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SpinController : ControllerBase
{
    private readonly ISpinService _spinService;
    
    public SpinController(ISpinService spinService)
    {
        _spinService = spinService;
    }
    
    [Authorize]
    [HttpPost("spin")]
    public async Task<IActionResult> CreateNewSpin()
    {
        var spin=await _spinService.CreateNewSpin();
        if (spin == null)
        {
            return NotFound("spin is not exists");
        }
        return Ok(spin);
    }

    [Authorize]
    [HttpGet("nums")]
    public async Task<IActionResult> GetHotAndColdNumbers()
    {
        var nums=await _spinService.GetHotAndColdNumberFromSpins();
        return Ok(nums);
    }
}