using CasinoRoulette.DTO;
using CasinoRoulette.Models;
using CasinoRoulette.Repositories;

namespace CasinoRoulette.Services;

public interface IPaymentService
{
    Task<bool> ProcessPayment(PaymentDto payment, int playerId);
}

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;

    public PaymentService(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<bool> ProcessPayment(PaymentDto payment, int playerId)
    {
        
        var player = await _paymentRepository.GetPlayer(playerId);
        if (!PlayerExists(player))
        {
            return false;
        }
        var transaction = new AccountTransaction()
        {
            Amount = payment.Amount,
            Type = payment.Type,
            PaymentMethod = payment.PaymentMethod,
            PlayerId = playerId
        };
        player.AccountBalance += payment.Amount;

        await _paymentRepository.AddTransaction(transaction);
        await _paymentRepository.SaveChanges();

        return true;
        
    }

    private static bool PlayerExists(Player? player)
    {
        if (player != null)
        {
            return true;
        }

        return false;
    }
}