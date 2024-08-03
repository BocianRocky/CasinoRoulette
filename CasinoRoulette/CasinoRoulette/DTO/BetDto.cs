
namespace CasinoRoulette.DTO;

public class BetDto
{
    public int SpinId { get; set; }
    public int GameId { get; set; }
    public decimal BetAmount { get; set; }
    public string BetType { get; set; }
    public List<BetNumberDto> BetNumbers { get; set; }
    
    
}