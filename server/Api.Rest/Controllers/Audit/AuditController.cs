using System.ComponentModel.DataAnnotations;
using Application.Interfaces;
using Application.Models.Constants;
using Application.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rest.Controllers.Audit;

[ApiController]
public class AuditController(IAuditLogReader reader) : ControllerBase
{
    public const string ControllerRoute = "api/audit";

    [HttpGet(ControllerRoute)]
    [Authorize(Policy = PolicyConstants.CanViewAuditLog)]
    public async Task<ActionResult<AuditLogPageDto>> GetAuditLog(
        Guid? findingId = null,
        [Range(0, int.MaxValue)] int offset = 0,
        [Range(1, 100)] int limit = 25)
    {
        return Ok(await reader.GetPageAsync(findingId, offset, limit));
    }
}