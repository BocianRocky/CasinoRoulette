using CasinoRoulette.Context;
using CasinoRoulette.Models;

namespace CasinoRoulette.Repositories;

public interface IAccountRepository
{
    Task AddPlayer(Player user);
    Task SaveChanges();
    Task<Player> GetPlayerByRefreshToken(string refreshToken);
    Task<Player> GetPlayerByLogin(string login);
    Task<Player> GetPlayerById(int playerId);
}

public class AccountRepository :IAccountRepository
{
    private readonly MasterContext _context;

    public AccountRepository(MasterContext context)
    {
        _context = context;
    }

    public async Task AddPlayer(Player user)
    {
        _context.Players.Add(user);
    }
    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
    public async Task<Player> GetPlayerByRefreshToken(string refreshToken)
    {
        return _context.Players.FirstOrDefault(u => u.RefreshToken == refreshToken);
    }
    public async Task<Player> GetPlayerByLogin(string login)
    {
        return _context.Players.FirstOrDefault(u => u.Login == login);
    }

    public async Task<Player> GetPlayerById(int playerId)
    {
        return _context.Players.FirstOrDefault(p => p.PlayerId == playerId);
    }

    
    
    
    
    
}