using Core.Domain.Entities;

namespace Application.Models.Dtos;

public class FindingDto
{
    public Guid FindingId { get; set; }

    public string Title { get; set; } = null!;

    public string Details { get; set; } = null!;

    public string AffectedAsset { get; set; } = null!;

    public string? Severity { get; set; }

    public string Status { get; set; } = null!;

    public DateOnly? DueDate { get; set; }

    public Guid? CreatorId { get; set; }

    public string? ReporterEmail { get; set; }


    public static Finding ToFinding(FindingDto finding)
    {
        return new Finding()
        {
            FindingId = finding.FindingId,
            CreatorId = finding.CreatorId != null ? finding.CreatorId : null,
            ReporterEmail = finding.ReporterEmail != null ? finding.ReporterEmail : null,
            Severity = finding.Severity != null ? finding.Severity : null,
            DiscoveryDate = DateOnly.FromDateTime(DateTime.Today),
            CreatedAt = DateTime.Now,
            Title = finding.Title,
            AffectedAsset = finding.AffectedAsset,
            Details = finding.Details,
            Status = finding.Status,
            DueDate = finding.DueDate != null ? finding.DueDate : DateOnly.FromDateTime(DateTime.Today.AddDays(30))
        };
    }
}