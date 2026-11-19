using Application.Models.Dtos;

namespace Application.Interfaces;

public interface IUserRoleService
{
    Task<ManagedUsersPageDto> ListUsersAsync(string actorId, int offset, int limit);
    Task<bool> ChangeRoleAsync(string actorId, string userId, string role);
}
