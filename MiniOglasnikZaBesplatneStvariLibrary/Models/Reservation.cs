using System;
using System.Collections.Generic;

namespace MiniOglasnikZaBesplatneStvariLibrary.Models;

public partial class Reservation
{
    public int Idreservation { get; set; }

    public DateTime ReservationDate { get; set; }

    public int? ItemId { get; set; }

    public int? UserDetailId { get; set; }

    public string Status { get; set; } = null!;

    public virtual Item? Item { get; set; }

    public virtual UserDetail? UserDetail { get; set; }
}
