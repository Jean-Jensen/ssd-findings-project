namespace Application.Models.Dtos;

public sealed record ManagedUserDto(string Id, string Username, IReadOnlyList<string> Roles);
