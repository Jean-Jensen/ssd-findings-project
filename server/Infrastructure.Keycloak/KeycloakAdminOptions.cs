namespace Infrastructure.Keycloak;

public sealed record KeycloakAdminOptions(
    string? Authority, string? AppClientId, string? ServiceClientId, string? ServiceClientSecret);
