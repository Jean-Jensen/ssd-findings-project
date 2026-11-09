using Core.Domain.Entities;

namespace Application.Models.Dtos;

public class FindingDto
{
    public string Findingid { get; set; } = null!;

    public string? Creatorid { get; set; }

    public string Details { get; set; } = null!;


    public static Finding ToFinding(FindingDto finding)
    {
        return new Finding()
        {
            Findingid = finding.Findingid,
            Creatorid = finding.Creatorid,
            Discoverydate = DateOnly.FromDateTime(DateTime.Today),
            Details = finding.Details

        };
    }
}