using Application.Models.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rest.Controllers.ForTestPurpose;

[ApiController]
[Route("api/test")]
public class ControllerForTesting : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = PolicyConstants.CanUpdateFindings)]
    public IActionResult Get()
    {
        return Ok("Test endpoint is working.");
    }
}