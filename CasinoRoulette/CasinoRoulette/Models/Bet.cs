using System;
using System.Collections.Generic;

namespace CasinoRoulette.Models;

public partial class Bet
{
    public int BetId { get; set; }

    public int SpinId { get; set; }

    public int PlayerId { get; set; }

    public int GameId { get; set; }

    public decimal BetAmount { get; set; }

    public int? Result { get; set; }

    public string BetType { get; set; } = null!;

    public virtual ICollection<BetNumber> BetNumbers { get; set; } = new List<BetNumber>();

    public virtual Game Game { get; set; } = null!;

    public virtual Player Player { get; set; } = null!;

    public virtual Spin Spin { get; set; } = null!;
}
