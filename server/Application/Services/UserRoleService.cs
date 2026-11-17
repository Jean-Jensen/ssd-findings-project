using System.ComponentModel.DataAnnotations;
using Application.Interfaces;
using Application.Models.Constants;
using Application.Models.Dtos;

namespace Application.Services;

internal sealed class UserRoleService(IKeycloakAdminClient keycloak) : IUserRoleService
{
    public async Task<ManagedUsersPageDto> ListUsersAsync(string actorId, int offset, int limit)
    {
        await RequireCurrentPmAsync(actorId);
        var users = await keycloak.GetUsersAsync(offset, limit + 1);

        var managedUsers = new List<ManagedUserDto>();
        foreach (var user in users.Take(limit))
        {
            if (user.Id == actorId || IsServiceAccount(user))
                continue;

            var roles = await keycloak.GetRoleMappingsAsync(user.Id);
            var appRoles = roles.Select(role => role.Name).Where(RoleConstants.IsAppRole).ToArray();
            managedUsers.Add(new ManagedUserDto(user.Id, user.Username, appRoles));
        }

        return new ManagedUsersPageDto(managedUsers, users.Length > limit);
    }

    public async Task<bool> ChangeRoleAsync(string actorId, string userId, string role)
    {
        if (!RoleConstants.IsAppRole(role))
            throw new ValidationException("Select a valid application role.");

        if (actorId == userId)
            throw new UnauthorizedAccessException("You cannot change your own role.");

        await RequireCurrentPmAsync(actorId);
        var user = await keycloak.FindUserAsync(userId);
        if (user is null || IsServiceAccount(user))
            return false;

        var currentRoles = await keycloak.GetRoleMappingsAsync(user.Id);
        var newRole = currentRoles.Any(current => current.Name == role) ? null : await keycloak.GetRoleAsync(role);
        var oldRoles = currentRoles.Where(current =>
            current.Name != role && RoleConstants.IsAppRole(current.Name)).ToArray();

        if (oldRoles.Length > 0)
            await keycloak.RemoveRolesAsync(user.Id, oldRoles);

        if (newRole is not null)
            await keycloak.AddRoleAsync(user.Id, newRole);

        return true;
    }

    private async Task RequireCurrentPmAsync(string actorId)
    {
        // An existing sign-in cookie can still contain a PM claim after demotion.
        if (await keycloak.FindUserAsync(actorId) is null ||
            !(await keycloak.GetRoleMappingsAsync(actorId)).Any(role => role.Name == RoleConstants.PM))
        {
            throw new UnauthorizedAccessException("A current PM role is required.");
        }
    }

    private static bool IsServiceAccount(KeycloakUserDto user) =>
        user.Username.StartsWith("service-account-", StringComparison.Ordinal);
}
