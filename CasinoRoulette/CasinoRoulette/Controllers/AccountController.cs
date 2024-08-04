
using CasinoRoulette.DTO;
using CasinoRoulette.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasinoRoulette.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }
    
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterStudent(RegisterDto model)
    {
        var register=await _accountService.RegisterPlayer(model);
        if (!register)
        {
            return BadRequest("Registration failed");
        }
        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login(LoginDto loginRequest)
    {

        var tokens = _accountService.LoginPlayer(loginRequest);
        if (tokens == null)
        {
            return BadRequest("Invalid username or password");
        }
        return Ok(tokens);
    }

    [Authorize(AuthenticationSchemes = "IgnoreTokenExpirationScheme")]
    [HttpPost("refresh")]
    public IActionResult Refresh(RefreshTokenRequest refreshToken)
    {
        var tokens = _accountService.RefreshTokenPlayer(refreshToken);
        if (tokens == null)
        {
            return Unauthorized("Invalid or expired refresh token");
        }
        return Ok(tokens);
    }
    
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDataPlayer(int id)
    {
        var dataPlayer =await _accountService.GetDataPlayerById(id);
        if (dataPlayer == null)
        {
            return Unauthorized("player not found");
        }
        return Ok(dataPlayer);
    }
    
    
}