using System;
using System.Collections.Generic;

namespace MiniOglasnikZaBesplatneStvariLibrary.Models;

public partial class ItemType
{
    public int IditemType { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Item> Items { get; } = new List<Item>();
}
