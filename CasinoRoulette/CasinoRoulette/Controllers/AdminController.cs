using CasinoRoulette.DTO;
using CasinoRoulette.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CasinoRoulette.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }
    [AllowAnonymous] //temporary
    [HttpGet("profit")]
    public async Task<IActionResult> ShowPlayersStatistics()
    {
        var players = await _adminService.GetDataPlayersAndProfit();
        if (!PlayersExists(players))
        {
            return NotFound("No players data found");
        }
        return Ok(players);
    }
    private static bool PlayersExists(List<PlayerProfitDto> players)
    {
        if (players == null)
        {
            return false;
        }

        return true;
    } 
}