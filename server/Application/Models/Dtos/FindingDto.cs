using Core.Domain.Entities;

namespace Application.Models.Dtos;

public class FindingDto
{
    public string Findingid { get; set; } = null!;

    public string? Creatorid { get; set; }

    public string Details { get; set; } = null!;

    public DateOnly Discoverydate { get; set; }

    public static Finding ToFinding(FindingDto finding)
    {
        return new Finding()
        {
            Findingid = finding.Findingid,
            Creatorid = finding.Creatorid,
            Discoverydate = finding.Discoverydate,
            Details = finding.Details

        };
    }
}