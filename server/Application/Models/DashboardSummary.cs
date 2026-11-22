namespace Application.Models;

public class DashboardCount
{
    public string Label { get; set; } = null!;
    public int Count { get; set; }
}

public class DashboardSummary
{
    public int TotalOpen { get; set; }
    
    public int Overdue { get; set; }
    public List<DashboardCount> BySeverity { get; set; } = new();
    public List<DashboardCount> ByStatus { get; set; } = new();
}