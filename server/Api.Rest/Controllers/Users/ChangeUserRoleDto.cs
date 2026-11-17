using System.ComponentModel.DataAnnotations;

namespace Api.Rest.Controllers.Users;

public sealed record ChangeUserRoleDto([Required] string Role);
