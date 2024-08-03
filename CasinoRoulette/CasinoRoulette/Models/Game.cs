using System;
using System.Collections.Generic;

namespace CasinoRoulette.Models;

public partial class Game
{
    public int GameId { get; set; }

    public string GameName { get; set; } = null!;

    public virtual ICollection<Bet> Bets { get; set; } = new List<Bet>();

    public virtual ICollection<Spin> Spins { get; set; } = new List<Spin>();
}
