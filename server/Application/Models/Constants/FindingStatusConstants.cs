namespace Application.Models.Constants;

public static class FindingStatusConstants
{
    public const string New = "New";
    public const string Triaged = "Triaged";
    public const string InProgress = "In Progress";
    public const string Remediated = "Remediated";
    public const string AcceptedRisk = "Accepted Risk";

    public static readonly List<string> Open = new() { New, Triaged, InProgress };
}