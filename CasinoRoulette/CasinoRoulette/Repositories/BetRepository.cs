using CasinoRoulette.Context;
using CasinoRoulette.Models;
using Microsoft.EntityFrameworkCore;

namespace CasinoRoulette.Repositories;

public interface IBetRepository
{
    Task<List<Bet>> GetBetsFromSpin(int spinId);
    Task<Bet> FindBet(List<Bet> playerBets, int betId);
    Task SaveChanges();
    Task<Player> GetPlayerById(int playerId);
    Task<bool> GameExists(int gameId);
    Task AddBet(Bet bet);
    Task AddBetNumber(BetNumber betNum);
}

public class BetRepository : IBetRepository
{
    private readonly MasterContext _context;

    public BetRepository(MasterContext context)
    {
        _context = context;
    }

    public async Task<List<Bet>> GetBetsFromSpin(int spinId)
    {
        var playerBets = await _context.Bets
            .Where(s => s.SpinId == spinId)
            .Include(b => b.BetNumbers)
            .ToListAsync();
        return playerBets;
    }

    public async Task<Bet> FindBet(List<Bet>playerBets, int betId)
    {
        return playerBets.FirstOrDefault(b => b.BetId == betId);
    }
    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
    public async Task<Player> GetPlayerById(int playerId)
    {
        return await _context.Players.FirstOrDefaultAsync(p => p.PlayerId == playerId);
    }

    public async Task<bool> GameExists(int gameId)
    {
        return await _context.Games.AnyAsync(g => g.GameId == gameId);
    }

    public async Task AddBet(Bet bet)
    {
        _context.Bets.Add(bet);
    }

    public async Task AddBetNumber(BetNumber betNum)
    {
        _context.BetNumbers.Add(betNum);
    }

    
}