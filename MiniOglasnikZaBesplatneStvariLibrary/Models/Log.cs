using System;
using System.Collections.Generic;

namespace MiniOglasnikZaBesplatneStvariLibrary.Models;

public partial class Log
{
    public int LogId { get; set; }

    public DateTime? Timestamp { get; set; }

    public string? Level { get; set; }

    public string? Message { get; set; }
}
