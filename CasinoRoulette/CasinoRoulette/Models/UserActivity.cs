using System;
using System.Collections.Generic;

namespace CasinoRoulette.Models;

public partial class UserActivity
{
    public int Uaid { get; set; }

    public int PlayerId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public string Ipaddress { get; set; } = null!;

    public virtual Player Player { get; set; } = null!;
}
