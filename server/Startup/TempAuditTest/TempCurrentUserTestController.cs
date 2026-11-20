using Application.Interfaces;
using Application.Models.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TempAuditTest;

[ApiController]
[Route("api/temp-current-user")]
public class TempCurrentUserTestController(ICurrentUser currentUser) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = PolicyConstants.CanViewFindings)]
    public IActionResult Get() => Ok(new { currentUser.UserId, currentUser.Email });
}