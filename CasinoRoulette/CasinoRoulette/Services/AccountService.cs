using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CasinoRoulette.Context;
using CasinoRoulette.DTO;
using CasinoRoulette.Helpers;
using CasinoRoulette.Models;
using CasinoRoulette.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace CasinoRoulette.Services;

public interface IAccountService
{
    Task<bool> RegisterPlayer(RegisterDto model);
    Task<TokensDto> LoginPlayer(LoginDto loginRequest);

    Task<TokensDto> RefreshTokenPlayer(RefreshTokenRequest refreshToken);
    Task<DataPlayerDto> GetDataPlayerById(int playedId);
}

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly MasterContext _context;
    private readonly IConfiguration _configuration;

    public AccountService(IConfiguration configuration,IAccountRepository accountRepository, MasterContext context)
    {
        _accountRepository = accountRepository;
        _context = context;
        _configuration = configuration;
    }

    public async Task<bool> RegisterPlayer(RegisterDto model)
    {
        var hashedPasswordAndSalt = SecurityHelpers.GetHashedPasswordAndSalt(model.Password);
        
        var user = new Player()
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Login = model.Login,
            Telephone = model.Telephone,
            Password = hashedPasswordAndSalt.Item1,
            Salt = hashedPasswordAndSalt.Item2,
            AccountBalance = 0,
            RefreshToken = SecurityHelpers.GenerateRefreshToken(),
            RefreshTokenExp = DateTime.Now.AddDays(1)
        };

        await _accountRepository.AddPlayer(user);
        await _accountRepository.SaveChanges();
        return true;
    }

    public async Task<TokensDto> LoginPlayer(LoginDto loginRequest)
    {
        Player user =await _accountRepository.GetPlayerByLogin(loginRequest.Login);
        
        if (user == null)
        {
            throw new Exception("");
        }

        string passwordHashFromDb = user.Password;
        string curHashedPassword = SecurityHelpers.GetHashedPasswordWithSalt(loginRequest.Password, user.Salt);

        if (passwordHashFromDb != curHashedPassword)
        {
            throw new Exception("");
        }

        Claim[] userclaims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.PlayerId.ToString()), 
            new Claim(ClaimTypes.Name, user.Login), 
            new Claim(ClaimTypes.Role, "user"),
            new Claim(ClaimTypes.Role, "admin")
        };

        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: "https://localhost:5231",
            audience: "https://localhost:5231",
            claims: userclaims,
            expires: DateTime.Now.AddMinutes(10),
            signingCredentials: creds
        );

        user.RefreshToken = SecurityHelpers.GenerateRefreshToken();
        user.RefreshTokenExp = DateTime.Now.AddDays(1);
        _context.SaveChanges();
        return new TokensDto
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = user.RefreshToken
        };
    }

    public async Task<TokensDto> RefreshTokenPlayer(RefreshTokenRequest refreshToken)
    {
        Player user = await _accountRepository.GetPlayerByRefreshToken(refreshToken.RefreshToken);
        if (user == null)
        {
            throw new SecurityTokenException("Invalid refresh token");
        }

        if (user.RefreshTokenExp < DateTime.Now)
        {
            throw new SecurityTokenException("Refresh token expired");
        }

        Claim[] userclaim = new[]
        {
            new Claim(ClaimTypes.Name, "bociek"),
            new Claim(ClaimTypes.Role, "user"),
            new Claim(ClaimTypes.Role, "admin")
        };

        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));

        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken jwtToken = new JwtSecurityToken(
            issuer: "https://localhost:5231",
            audience: "https://localhost:5231",
            claims: userclaim,
            expires: DateTime.Now.AddMinutes(10),
            signingCredentials: creds
        );

        user.RefreshToken = SecurityHelpers.GenerateRefreshToken();
        user.RefreshTokenExp = DateTime.Now.AddDays(1);
        await _accountRepository.SaveChanges();
        return new TokensDto
        { 
        AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken),
        RefreshToken = user.RefreshToken
        };
    }

    public async Task<DataPlayerDto> GetDataPlayerById(int playerId)
    {
        var player =await _accountRepository.GetPlayerById(playerId);
        if (!PlayerExists(player))
        {
            throw new Exception("player doesn't exists");
        }
        var dataPlayer = new DataPlayerDto()
        {
            FirstName = player.FirstName,
            LastName = player.LastName,
            AccountBalance = player.AccountBalance
        };
        return dataPlayer;
    }
    private static bool PlayerExists(Player? player)
    {
        if (player != null)
        {
            return true;
        }

        return false;
    }
    
}