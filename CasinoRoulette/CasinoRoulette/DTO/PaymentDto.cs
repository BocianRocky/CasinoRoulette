namespace CasinoRoulette.DTO;

public class PaymentDto
{
    public decimal Amount { get; set; }
    public string Type { get; set; }
    public string PaymentMethod { get; set; }
}