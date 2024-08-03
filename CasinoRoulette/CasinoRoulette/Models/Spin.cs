using System;
using System.Collections.Generic;

namespace CasinoRoulette.Models;

public partial class Spin
{
    public int SpinId { get; set; }

    public int? NumberWinner { get; set; }

    public int GameId { get; set; }

    public virtual ICollection<Bet> Bets { get; set; } = new List<Bet>();

    public virtual Game Game { get; set; } = null!;
}
