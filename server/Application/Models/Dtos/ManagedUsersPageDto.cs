namespace Application.Models.Dtos;

public sealed record ManagedUsersPageDto(IReadOnlyList<ManagedUserDto> Users, bool HasMore);
