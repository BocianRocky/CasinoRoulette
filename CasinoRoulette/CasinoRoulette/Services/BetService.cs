using CasinoRoulette.DTO;
using CasinoRoulette.Models;
using CasinoRoulette.Repositories;


namespace CasinoRoulette.Services;

public interface IBetService
{
    Task<AmountWonDto> AssignBetResults(RouletteResultDto resultDto, int playerId);
    Task<int> CreateBet(BetDto betDto, int playerId);
    Task<bool> DeleteBetsFromSpinAndPlayer(int spinId, int playerId);
}

public class BetService : IBetService
{
    private readonly ISpinRepository _spinRepository;
    private readonly IBetRepository _betRepository;
    private readonly IAccountRepository _accountRepository;

    public BetService(ISpinRepository spinRepository, IBetRepository betRepository, IAccountRepository accountRepository)
    {
        _spinRepository = spinRepository;
        _betRepository = betRepository;
        _accountRepository = accountRepository;
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
    
    

    public async Task<AmountWonDto> AssignBetResults(RouletteResultDto resultDto, int playerId)
    {
        decimal amountWon = 0;
        var spin = await _spinRepository.GetSpin(resultDto.SpinId);
        
        if (!SpinExists(spin))
        {
            throw new Exception("Spin doesn't exists");
        }
        var player = await _accountRepository.GetPlayerById(playerId);
        if (!PlayerExists(player))
        {
            throw new Exception("Player doesn't exists");
        }
        
        spin.NumberWinner = resultDto.numWinner;

        var playerBets = await _betRepository.GetBetsFromSpinAndPlayer(spin.SpinId, player.PlayerId);
             
        var betWithNumbers = playerBets.Select(bet => new BetWithNumbersDto()
        {
            BetId = bet.BetId,
            BetNumbers = bet.BetNumbers.Select(n => n.Number).ToList()
        }).ToList();
             
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
                amountWon+=CalculateByBetType(bet.BetAmount,bet.BetType);  
            }
            else
            {
                bet.Result = 0;
            }
        }

        player.AccountBalance += amountWon;
        var win = new AmountWonDto()
        {
            AmountWon = amountWon
        };
        
        await _betRepository.SaveChanges();

        return win;
    }

    public async Task<bool> DeleteBetsFromSpinAndPlayer(int spinId, int playerId)
    {
        var bets =await _spinRepository.GetBetsFromSpinAndPlayer(spinId, playerId);
        var numbersToRemove = await _betRepository.FindNumbersFromBet(bets);
        await _betRepository.RemoveNumbers(numbersToRemove);
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
