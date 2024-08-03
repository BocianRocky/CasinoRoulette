using System;
using System.Collections.Generic;

namespace CasinoRoulette.Models;

public partial class AccountTransaction
{
    public int TransactionId { get; set; }

    public decimal Amount { get; set; }

    public string Type { get; set; } = null!;

    public string PaymentMethod { get; set; } = null!;

    public int PlayerId { get; set; }

    public virtual Player Player { get; set; } = null!;
}
