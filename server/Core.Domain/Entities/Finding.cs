using System;
using System.Collections.Generic;

namespace Core.Domain.Entities;

public partial class Finding
{
    public Guid FindingId { get; set; }

    public string Title { get; set; } = null!;

    public string Details { get; set; } = null!;

    public string AffectedAsset { get; set; } = null!;

    public string? Severity { get; set; }

    public string Status { get; set; } = null!;

    public DateOnly DiscoveryDate { get; set; }

    public DateOnly? DueDate { get; set; }

    public Guid? CreatorId { get; set; }

    public string? ReporterEmail { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual UserLink? Creator { get; set; }

    public virtual ICollection<UserLink> Users { get; set; } = new List<UserLink>();
}
