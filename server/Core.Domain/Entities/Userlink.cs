using System;
using System.Collections.Generic;

namespace Core.Domain.Entities;

public partial class Userlink
{
    public string Userid { get; set; } = null!;

    public string? Email { get; set; }

    public virtual ICollection<Finding> Findings { get; set; } = new List<Finding>();
}
