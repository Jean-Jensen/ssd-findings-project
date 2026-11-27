using Application.Interfaces;
using Application.Models;
using Application.Models.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rest.Controllers.Dashboard;

[ApiController]
public class DashboardController(IDashboardService dashboard) : ControllerBase
{
    public const string ControllerRoute = "api/dashboard/";
    public const string SummaryRoute = ControllerRoute + "Summary";
    public const string ReportRoute = ControllerRoute + "Report";

    [HttpGet(SummaryRoute)]
    [Authorize(Policy = PolicyConstants.CanViewFindings)]
    public async Task<ActionResult<DashboardSummary>> GetSummary()
    {
        return Ok(await dashboard.GetSummaryAsync());
    }

    [HttpGet(ReportRoute)]
    [Authorize(Policy = PolicyConstants.CanViewFindings)]
    public async Task<ActionResult<List<FindingReportItem>>> GetReport(
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        [FromQuery] string? affectedAsset)
    {
        if (fromDate.HasValue && toDate.HasValue &&
            fromDate.Value > toDate.Value)
        {
            return BadRequest("From date cannot be later than to date.");
        }

        var report = await dashboard.GetReportAsync(
            fromDate,
            toDate,
            affectedAsset
        );

        return Ok(report);
    }
}