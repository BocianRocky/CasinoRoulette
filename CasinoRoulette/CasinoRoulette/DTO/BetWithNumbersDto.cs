namespace CasinoRoulette.DTO;

public class BetWithNumbersDto
{
    public int BetId { get; set; }
    public List<int?> BetNumbers { get; set; }
}