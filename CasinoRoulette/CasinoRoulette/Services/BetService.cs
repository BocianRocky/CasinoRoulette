using CasinoRoulette.DTO;
using CasinoRoulette.Models;
using CasinoRoulette.Repositories;


namespace CasinoRoulette.Services;

public interface IBetService
{
    Task<bool> AssignBetResults(RouletteResultDto resultDto);
    Task<int> CreateBet(BetDto betDto, int playerId);
}

public class BetService : IBetService
{
    private readonly ISpinRepository _spinRepository;
    private readonly IBetRepository _betRepository;

    public BetService(ISpinRepository spinRepository, IBetRepository betRepository)
    {
        _spinRepository = spinRepository;
        _betRepository = betRepository;
    }
    
    public async Task<int> CreateBet(BetDto betDto,int playerId)
    {


        var gameExists = await _betRepository.GameExists(betDto.GameId);
        if (!gameExists)
        {
            throw new Exception("game doesn't exists");
        }

        var player = await _betRepository.GetPlayerById(playerId);
        if (player.AccountBalance - betDto.BetAmount < 0)
        {
            throw new Exception("Account balance is not enough");
        }

        player.AccountBalance -= betDto.BetAmount;
        
        var bet = new Bet()
        {
            SpinId = betDto.SpinId,
            PlayerId = playerId,
            GameId = betDto.GameId,
            BetAmount = betDto.BetAmount,
            BetType = betDto.BetType,
            Result = null 
        };


        await _betRepository.AddBet(bet);
        await _betRepository.SaveChanges();

        
        int betId = bet.BetId;
        
        foreach (var num in betDto.BetNumbers)
        {
            var betNum = new BetNumber()
            {
                BetId = betId,
                Number = num.Number
            };

            await _betRepository.AddBetNumber(betNum);
        }

        await _betRepository.SaveChanges();

        return betId;
    }
    
    

    public async Task<bool> AssignBetResults(RouletteResultDto resultDto)
    {
        var spin = await _spinRepository.GetSpin(resultDto.SpinId);
        if (!SpinExists(spin))
        {
            return false;
        }
        spin.NumberWinner = resultDto.numWinner;

        var playerBets = await _betRepository.GetBetsFromSpin(spin.SpinId);
             
        var betWithNumbers = playerBets.Select(bet => new BetWithNumbersDto()
        {
            BetId = bet.BetId,
            BetNumbers = bet.BetNumbers.Select(n => n.Number).ToList()
        }).ToList();
             
        var playerUpdates = new Dictionary<int, decimal>();
             
        foreach (var currBet in betWithNumbers)
        {
            var bet =await _betRepository.FindBet(playerBets, currBet.BetId);
             
            if (!BetExists(bet))
            {
                continue;
            }
             
            if (currBet.BetNumbers.Contains(spin.NumberWinner))
            {
                bet.Result = 1;
                if (!playerUpdates.ContainsKey(bet.PlayerId))
                {
                    playerUpdates[bet.PlayerId] = 0;
                }
             
                playerUpdates[bet.PlayerId] += CalculateByBetType(bet.BetAmount,bet.BetType);  
            }
            else
            {
                bet.Result = 0;
            }
        }
             
        foreach (var update in playerUpdates)
        {
            var player =await  _betRepository.GetPlayerById(update.Key);
            if (PlayerExists(player))
            {
                player.AccountBalance += update.Value;
            }
        }

        await _betRepository.SaveChanges();

        return true;
    }
    

    private static decimal CalculateByBetType(decimal currAmount, string type)
    {
        decimal amount=0;
        if (Enum.TryParse(type, true, out TypeOfBet betType))
        {
            int multiplier = (int)betType+1;
            amount = currAmount * multiplier;
        }
        else
        {
            throw new ArgumentException("Invalid bet type string.");
        }

        return amount;
    }

    private static bool PlayerExists(Player? player)
    {
        if (player != null)
        {
            return true;
        }

        return false;
    }

    private static bool BetExists(Bet? bet)
    {
        if (bet != null)
        {
            return true;
        }

        return false;
    }

    private static bool SpinExists(Spin? spin)
    {
        if (spin != null)
        {
            return true;
        }

        return false;
    }
}
