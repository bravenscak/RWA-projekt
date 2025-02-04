using System;
using System.Collections.Generic;

namespace MiniOglasnikZaBesplatneStvariLibrary.Models;

public partial class Tag
{
    public int Idtag { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ItemTag> ItemTags { get; } = new List<ItemTag>();
}
