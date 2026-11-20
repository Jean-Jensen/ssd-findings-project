using System;
using System.Collections.Generic;

namespace Core.Domain.Entities;

public partial class AuditLog
{
    public long AuditId { get; set; }

    public DateTime OccurredAt { get; set; }

    public Guid FindingId { get; set; }

    public Guid? ActorId { get; set; }

    public string ActorEmail { get; set; } = null!;

    public string Action { get; set; } = null!;

    public string? FieldName { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public virtual Finding Finding { get; set; } = null!;
}
