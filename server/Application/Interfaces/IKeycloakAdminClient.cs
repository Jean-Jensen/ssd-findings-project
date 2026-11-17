using Application.Models.Dtos;

namespace Application.Interfaces;

public interface IKeycloakAdminClient
{
    Task<KeycloakUserDto[]> GetUsersAsync(int offset, int limit);
    Task<KeycloakUserDto?> FindUserAsync(string userId);
    Task<KeycloakRoleDto[]> GetRoleMappingsAsync(string userId);
    Task<KeycloakRoleDto> GetRoleAsync(string role);
    Task RemoveRolesAsync(string userId, KeycloakRoleDto[] roles);
    Task AddRoleAsync(string userId, KeycloakRoleDto role);
}
