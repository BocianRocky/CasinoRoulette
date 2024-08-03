using System;
using System.Collections.Generic;

namespace CasinoRoulette.Models;

public partial class BetNumber
{
    public int BetNumberId { get; set; }

    public int BetId { get; set; }

    public int? Number { get; set; }

    public virtual Bet Bet { get; set; } = null!;
}
