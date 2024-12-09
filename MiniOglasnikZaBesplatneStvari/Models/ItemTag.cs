using System;
using System.Collections.Generic;

namespace MiniOglasnikZaBesplatneStvari.Models;

public partial class ItemTag
{
    public int IditemTag { get; set; }

    public int? ItemId { get; set; }

    public int? TagId { get; set; }

    public virtual Item? Item { get; set; }

    public virtual Tag? Tag { get; set; }
}
