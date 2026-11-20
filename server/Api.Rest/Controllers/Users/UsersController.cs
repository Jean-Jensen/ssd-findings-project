using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Application.Interfaces;
using Application.Models.Constants;
using Application.Models.Dtos;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rest.Controllers.Users;

[ApiController]
[Route("api/users")]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme,
    Policy = PolicyConstants.CanManageUserRoles)]
public class UsersController(IUserRoleService userRoles) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ManagedUsersPageDto>> ListManagedUsers(
        [Range(0, int.MaxValue)] int offset = 0, [Range(1, 100)] int limit = 25)
    {
        var actorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(actorId))
            return Forbid(CookieAuthenticationDefaults.AuthenticationScheme);

        var usersList = await userRoles.ListUsersAsync(actorId, offset, limit);
        return Ok(usersList);
    }

    [HttpPut("{userId}/role")]
    public async Task<IActionResult> ChangeUserRole(string userId, ChangeUserRoleDto request)
    {
        var actorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(actorId))
            return Forbid(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!await userRoles.ChangeRoleAsync(actorId, userId, request.Role))
            return NotFound(new ProblemDetails { Status = StatusCodes.Status404NotFound, Title = "User not found." });

        return NoContent();
    }
}
