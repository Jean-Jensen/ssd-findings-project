using Application.Models;

namespace Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummary> GetSummaryAsync();
}