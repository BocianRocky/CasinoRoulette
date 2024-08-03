using CasinoRoulette.Context;
using CasinoRoulette.Models;
using Microsoft.EntityFrameworkCore;

namespace CasinoRoulette.Repositories;

public interface ISpinRepository
{
    Task AddSpin(Spin spin);
    Task SaveChanges();
    Task<Spin> GetSpin(int spinId);
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
    
    
    
}