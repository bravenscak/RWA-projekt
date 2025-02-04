using System;
using System.Collections.Generic;

namespace MiniOglasnikZaBesplatneStvariLibrary.Models;

public partial class Item
{
    public int Iditem { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? TypeId { get; set; }

    public virtual ICollection<ItemTag> ItemTags { get; } = new List<ItemTag>();

    public virtual ICollection<Reservation> Reservations { get; } = new List<Reservation>();

    public virtual ItemType? Type { get; set; }
}
