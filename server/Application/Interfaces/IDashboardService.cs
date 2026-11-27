using Application.Models;

namespace Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummary> GetSummaryAsync();
    
    Task<List<FindingReportItem>> GetReportAsync(
        DateOnly? fromDate,
        DateOnly? toDate,
        string? affectedAsset
    );
}