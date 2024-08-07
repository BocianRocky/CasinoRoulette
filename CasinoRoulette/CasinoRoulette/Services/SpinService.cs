using CasinoRoulette.DTO;
using CasinoRoulette.Models;
using CasinoRoulette.Repositories;

namespace CasinoRoulette.Services;

public interface ISpinService
{
    Task<Spin> CreateNewSpin();
    Task<HotAndColdNumbersDto> GetHotAndColdNumberFromSpins();
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

    public async Task<HotAndColdNumbersDto> GetHotAndColdNumberFromSpins()
    {
        var nums =await _spinRepository.GetWinnerNumbers();

        if (nums == null || nums.Count<3)
        {
            nums = new List<int>();
            var random = new Random();
            for (int i = 0; i < 3; i++)
            {
                nums.Add(random.Next(0,37));
            }
        }
        
        var hotNums = nums.Take(3).ToList();
        var coldNums = nums.TakeLast(3).ToList();

        var hotAndCold = new HotAndColdNumbersDto()
        {
            HotNumbers = hotNums,
            ColdNumbers = coldNums
        };
        return hotAndCold;
    }
    
    
    
    
}