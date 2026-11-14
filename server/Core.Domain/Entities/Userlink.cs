using System;
using System.Collections.Generic;

namespace Core.Domain.Entities;

public partial class UserLink
{
    public Guid UserId { get; set; }

    public string Email { get; set; } = null!;

    public string? DisplayName { get; set; }

    public virtual ICollection<Finding> Findings { get; set; } = new List<Finding>();

    public virtual ICollection<Finding> FindingsNavigation { get; set; } = new List<Finding>();
}
