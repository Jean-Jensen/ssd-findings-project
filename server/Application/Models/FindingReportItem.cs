namespace Application.Models;

public class FindingReportItem
{
    public Guid FindingId { get; set; }
    public string Title { get; set; } = null!;
    public string Details { get; set; } = null!;
    public string AffectedAsset { get; set; } = null!;
    public string? Severity { get; set; }
    public string Status { get; set; } = null!;
    public DateOnly DiscoveryDate { get; set; }
    public DateOnly? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
}