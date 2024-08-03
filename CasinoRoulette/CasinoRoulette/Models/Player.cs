using System;
using System.Collections.Generic;

namespace CasinoRoulette.Models;

public partial class Player
{
    public int PlayerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Telephone { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Salt { get; set; } = null!;

    public decimal AccountBalance { get; set; }

    public string RefreshToken { get; set; } = null!;

    public DateTime RefreshTokenExp { get; set; }

    public virtual ICollection<AccountTransaction> AccountTransactions { get; set; } = new List<AccountTransaction>();

    public virtual ICollection<Bet> Bets { get; set; } = new List<Bet>();

    public virtual ICollection<UserActivity> UserActivities { get; set; } = new List<UserActivity>();
}
