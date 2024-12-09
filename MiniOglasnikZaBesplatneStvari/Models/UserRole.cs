using System;
using System.Collections.Generic;

namespace MiniOglasnikZaBesplatneStvari.Models;

public partial class UserRole
{
    public int IduserRole { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<UserDetail> UserDetails { get; } = new List<UserDetail>();
}
