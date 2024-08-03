using CasinoRoulette.Context;
using CasinoRoulette.Models;
using Microsoft.EntityFrameworkCore;

namespace CasinoRoulette.Repositories;

public interface IPaymentRepository
{
    Task<Player> GetPlayer(int playerId);
    Task AddTransaction(AccountTransaction transaction);
    Task SaveChanges();

}

public class PaymentRepository : IPaymentRepository
{
    private readonly MasterContext _context;

    public PaymentRepository(MasterContext context)
    {
        _context = context;
    }

    public async Task<Player> GetPlayer(int playerId)
    {
        return await _context.Players.FirstOrDefaultAsync(p => p.PlayerId == playerId);
    }

    public async Task AddTransaction(AccountTransaction transaction)
    {
        await _context.AccountTransactions.AddAsync(transaction);
    }
    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
    
   
}