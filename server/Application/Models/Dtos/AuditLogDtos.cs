namespace Application.Models.Dtos;

public class AuditLogEntryDto
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
}

public class AuditLogPageDto
{
    public List<AuditLogEntryDto> Items { get; set; } = new();
    public int Total { get; set; }
}