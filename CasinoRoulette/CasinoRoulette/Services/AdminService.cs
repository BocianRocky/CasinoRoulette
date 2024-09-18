using CasinoRoulette.DTO;
using CasinoRoulette.Repositories;

namespace CasinoRoulette.Services;

public interface IAdminService
{
    Task<List<PlayerProfitDto>> GetDataPlayersAndProfit();
}

public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepository;

    public AdminService(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public async Task<List<PlayerProfitDto>> GetDataPlayersAndProfit()
    {
        var players =await _adminRepository.GetAllPlayersAndProfit();
        if (!PlayersExists(players))
        {
            throw new InvalidOperationException("Error with player data");
        }
        return players;
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