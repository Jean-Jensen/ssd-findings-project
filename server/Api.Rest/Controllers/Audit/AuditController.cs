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
        string? findingId = null,
        [Range(0, int.MaxValue)] int offset = 0,
        [Range(1, 100)] int limit = 25)
    {
        Guid? parsedFindingId = null;

        // Validate the finding ID format.
        if (!string.IsNullOrWhiteSpace(findingId))
        {
            if (!Guid.TryParse(findingId.Trim(), out var parsedId))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid finding ID",
                    Detail = "The finding ID must be a valid GUID."
                });
            }

            parsedFindingId = parsedId;

            // Check whether the finding exists.
            var findingExists = await reader.FindingExistsAsync(parsedId);

            if (!findingExists)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Finding not found",
                    Detail = $"No finding exists with ID '{parsedId}'."
                });
            }
        }

        var result = await reader.GetPageAsync(
            parsedFindingId,
            offset,
            limit
        );

        return Ok(result);
    }
}