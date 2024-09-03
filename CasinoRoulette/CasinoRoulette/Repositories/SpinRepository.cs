using CasinoRoulette.Context;
using CasinoRoulette.Models;
using Microsoft.EntityFrameworkCore;

namespace CasinoRoulette.Repositories;

public interface ISpinRepository
{
    Task AddSpin(Spin spin);
    Task SaveChanges();
    Task<Spin> GetSpin(int spinId);
    Task<List<int>> GetWinnerNumbers();
    Task<List<int>> GetBetsFromSpinAndPlayer(int spinId, int playerId);
}
public class SpinRepository : ISpinRepository
{
    private readonly MasterContext _context;

    public SpinRepository(MasterContext context)
    {
        _context = context;
    }

    public async Task AddSpin(Spin spin)
    {
        await _context.Spins.AddAsync(spin);
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<Spin> GetSpin(int spinId)
    {
        return await _context.Spins.FirstOrDefaultAsync(s => s.SpinId == spinId);
    }

    public async Task<List<int>> GetWinnerNumbers()
    {
        return await _context.Spins.GroupBy(nw => nw.NumberWinner)
            .OrderByDescending(g => g.Count()).Select(g => g.Key)
            .Where(nw=>nw.HasValue).Select(nw=>nw.Value)
            .ToListAsync();
    }

    public async Task<List<int>>GetBetsFromSpinAndPlayer(int spinId, int playerId)
    {
        return await _context.Bets.Where(sp => sp.SpinId == spinId && sp.PlayerId == playerId)
            .Select(b=>b.BetId).ToListAsync();
    }
    
    
    
    
}