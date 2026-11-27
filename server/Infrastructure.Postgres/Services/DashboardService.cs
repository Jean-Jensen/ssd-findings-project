
using Application.Interfaces;
using Application.Models;
using Application.Models.Constants;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostGres.Services;

public class DashboardService(AppDbContext db) : IDashboardService
{
    private const string Unrated = "Unrated";

    private static readonly string[] SeverityOrder =
    {
        SeverityConstants.Critical, SeverityConstants.High,
        SeverityConstants.Medium, SeverityConstants.Low, Unrated
    };

    public async Task<DashboardSummary> GetSummaryAsync()
    {
        var open = db.Findings.AsNoTracking()
            .Where(f => FindingStatusConstants.Open.Contains(f.Status));

        var severityCounts = await open
            .GroupBy(f => f.Severity)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync();

        var statusCounts = await open
            .GroupBy(f => f.Status)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var overdue = await open.CountAsync(f => f.DueDate < today);

        return new DashboardSummary
        {
            TotalOpen = statusCounts.Sum(s => s.Count),
            Overdue = overdue,
            BySeverity = SeverityOrder.Select(label => new DashboardCount
            {
                Label = label,
                Count = severityCounts
                    .Where(s => (s.Key ?? Unrated) == label)
                    .Sum(s => s.Count)
            }).ToList(),
            ByStatus = FindingStatusConstants.Open.Select(label => new DashboardCount
            {
                Label = label,
                Count = statusCounts
                    .Where(s => s.Key == label)
                    .Sum(s => s.Count)
            }).ToList()
        };
    }

    public async Task<List<FindingReportItem>> GetReportAsync(
        DateOnly? fromDate,
        DateOnly? toDate,
        string? affectedAsset)
    {
        var query = db.Findings.AsNoTracking().AsQueryable();

        if (fromDate.HasValue)
        {
            query = query.Where(f => f.DiscoveryDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(f => f.DiscoveryDate <= toDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(affectedAsset))
        {
            var assetFilter = affectedAsset.Trim().ToLower();

            query = query.Where(f =>
                f.AffectedAsset.ToLower().Contains(assetFilter));
        }

        return await query
            .OrderByDescending(f => f.DiscoveryDate)
            .Select(f => new FindingReportItem
            {
                FindingId = f.FindingId,
                Title = f.Title,
                Details = f.Details,
                AffectedAsset = f.AffectedAsset,
                Severity = f.Severity,
                Status = f.Status,
                DiscoveryDate = f.DiscoveryDate,
                DueDate = f.DueDate,
                CreatedAt = f.CreatedAt
            })
            .ToListAsync();
    }
}
