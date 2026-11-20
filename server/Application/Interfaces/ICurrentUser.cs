namespace Application.Interfaces;

public interface ICurrentUser
{
    /// Keycloak user id of the logged-in user, or null if nobody is logged in.
    Guid? UserId { get; }

    /// Email of the logged-in user, or null if nobody is logged in.
    string? Email { get; }
}