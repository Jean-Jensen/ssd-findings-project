using System;
using System.Collections.Generic;

namespace Core.Domain.Entities;

public partial class Finding
{
    public string Findingid { get; set; } = null!;

    public string? Creatorid { get; set; }

    public string Details { get; set; } = null!;

    public DateOnly Discoverydate { get; set; }

    public virtual ICollection<Userlink> Assigneds { get; set; } = new List<Userlink>();
}
