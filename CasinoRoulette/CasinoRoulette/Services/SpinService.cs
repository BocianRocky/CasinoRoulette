using CasinoRoulette.Models;
using CasinoRoulette.Repositories;

namespace CasinoRoulette.Services;

public interface ISpinService
{
    Task<Spin> CreateNewSpin();
}

public class SpinService : ISpinService
{
    private readonly ISpinRepository _spinRepository;

    public SpinService(ISpinRepository spinRepository)
    {
        _spinRepository = spinRepository;
    }

    public async Task<Spin> CreateNewSpin()
    {
        var newSpin = new Spin()
        {
            NumberWinner = null,
            GameId = 1
        };
        await _spinRepository.AddSpin(newSpin);
        await _spinRepository.SaveChanges();
        return newSpin;
    }
    
    
    
    
}