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

    [HttpGet(SummaryRoute)]
    [Authorize(Policy = PolicyConstants.CanViewFindings)]
    public async Task<ActionResult<DashboardSummary>> GetSummary()
    {
        return Ok(await dashboard.GetSummaryAsync());
    }
}