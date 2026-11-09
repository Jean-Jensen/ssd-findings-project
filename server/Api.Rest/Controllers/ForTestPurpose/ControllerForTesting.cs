using Application.Models.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rest.Controllers.ForTestPurpose;

[ApiController]
[Route("api/test")]
public class ControllerForTesting : ControllerBase
{
    [HttpGet("update-findings")]
    [Authorize(Policy = PolicyConstants.CanUpdateFindings)]
    public IActionResult Get() => Ok(PolicyConstants.CanUpdateFindings);

    [HttpGet("view-findings")]
    [Authorize(Policy = PolicyConstants.CanViewFindings)]
    public IActionResult ViewFindings() => Ok(PolicyConstants.CanViewFindings);

    [HttpGet("create-findings")]
    [Authorize(Policy = PolicyConstants.CanCreateFindings)]
    public IActionResult CreateFindings() => Ok(PolicyConstants.CanCreateFindings);

    [HttpGet("assign-findings")]
    [Authorize(Policy = PolicyConstants.CanAssignFindings)]
    public IActionResult AssignFindings() => Ok(PolicyConstants.CanAssignFindings);

    [HttpGet("set-sla-targets")]
    [Authorize(Policy = PolicyConstants.CanSetSlaTargets)]
    public IActionResult SetSlaTargets() => Ok(PolicyConstants.CanSetSlaTargets);

    [HttpGet("manage-user-roles")]
    [Authorize(Policy = PolicyConstants.CanManageUserRoles)]
    public IActionResult ManageUserRoles() => Ok(PolicyConstants.CanManageUserRoles);
}
