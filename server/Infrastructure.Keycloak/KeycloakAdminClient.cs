using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Application.Interfaces;
using Application.Models.Dtos;

namespace Infrastructure.Keycloak;

public sealed class KeycloakAdminClient(HttpClient httpClient, KeycloakAdminOptions options) : IKeycloakAdminClient
{
    private AdminConnection? connection;

    public async Task<KeycloakUserDto[]> GetUsersAsync(int offset, int limit)
    {
        var admin = await ConnectAsync();
        return await GetAsync<KeycloakUserDto[]>($"{admin.UsersUrl}?first={offset}&max={limit}&briefRepresentation=true");
    }

    public async Task<KeycloakUserDto?> FindUserAsync(string userId)
    {
        // Dot segments would be normalized before the request is sent.
        if (userId is "." or ".." || string.IsNullOrWhiteSpace(userId))
            return null;

        var admin = await ConnectAsync();
        using var response = await httpClient.GetAsync(admin.GetUserUrl(userId));
        if (response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<KeycloakUserDto>();
    }

    public async Task<KeycloakRoleDto[]> GetRoleMappingsAsync(string userId)
    {
        var admin = await ConnectAsync();
        return await GetAsync<KeycloakRoleDto[]>(admin.GetRoleMappingsUrl(userId));
    }

    public async Task<KeycloakRoleDto> GetRoleAsync(string role)
    {
        var admin = await ConnectAsync();
        return await GetAsync<KeycloakRoleDto>($"{admin.AppClientRolesUrl}/{Uri.EscapeDataString(role)}");
    }

    public async Task RemoveRolesAsync(string userId, KeycloakRoleDto[] roles)
    {
        var admin = await ConnectAsync();
        using var request = new HttpRequestMessage(HttpMethod.Delete, admin.GetRoleMappingsUrl(userId))
        {
            Content = JsonContent.Create(roles)
        };
        using var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task AddRoleAsync(string userId, KeycloakRoleDto role)
    {
        var admin = await ConnectAsync();
        using var response = await httpClient.PostAsJsonAsync(admin.GetRoleMappingsUrl(userId), new[] { role });
        response.EnsureSuccessStatusCode();
    }

    // Each service instance reuses one token and client ID for its Keycloak calls.
    private async Task<AdminConnection> ConnectAsync()
    {
        if (connection is not null)
            return connection;

        var authority = options.Authority?.TrimEnd('/');
        if (!Uri.TryCreate(authority, UriKind.Absolute, out var issuer) ||
            issuer.Scheme is not ("http" or "https") ||
            string.IsNullOrWhiteSpace(options.AppClientId) ||
            string.IsNullOrWhiteSpace(options.ServiceClientId) ||
            string.IsNullOrWhiteSpace(options.ServiceClientSecret))
        {
            throw new InvalidOperationException("Missing or invalid Keycloak role-management configuration.");
        }

        var issuerUrl = issuer.GetLeftPart(UriPartial.Path);
        // Turn /realms/{realm} into /admin/realms/{realm}, preserving any server path prefix.
        const string realmSegment = "/realms/";
        var realmStart = issuerUrl.LastIndexOf(realmSegment, StringComparison.Ordinal);
        if (realmStart < 0 || issuerUrl.IndexOf('/', realmStart + realmSegment.Length) >= 0)
            throw new InvalidOperationException("The OIDC authority must point to a Keycloak realm.");

        using var credentials = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = options.ServiceClientId,
            ["client_secret"] = options.ServiceClientSecret
        });
        using var tokenResponse = await httpClient.PostAsync($"{issuerUrl}/protocol/openid-connect/token", credentials);
        tokenResponse.EnsureSuccessStatusCode();
        var token = await tokenResponse.Content.ReadFromJsonAsync<JsonElement>();
        var accessToken = token.GetProperty("access_token").GetString();
        if (string.IsNullOrWhiteSpace(accessToken))
            throw new InvalidOperationException("Keycloak returned an empty access token.");

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var realmUrl = issuerUrl.Insert(realmStart, "/admin");
        var clientsUrl = $"{realmUrl}/clients";
        
        var clients = await GetAsync<KeycloakClientDto[]>($"{clientsUrl}?clientId={Uri.EscapeDataString(options.AppClientId)}");
        
        var appClient = clients.FirstOrDefault(client => client.ClientId == options.AppClientId);
        if (string.IsNullOrWhiteSpace(appClient?.Id))
            throw new InvalidOperationException("The service account cannot find the application client in Keycloak.");

        var appClientUuid = Uri.EscapeDataString(appClient.Id);
        var usersUrl = $"{realmUrl}/users";
        var appClientRolesUrl = $"{clientsUrl}/{appClientUuid}/roles";
        connection = new AdminConnection(usersUrl, appClientRolesUrl, appClientUuid);
        return connection;
    }

    private async Task<T> GetAsync<T>(string url)
    {
        var result = await httpClient.GetFromJsonAsync<T>(url);
        if (result is null)
            throw new InvalidOperationException("Keycloak returned an empty response.");

        return result;
    }

    private sealed record AdminConnection(string UsersUrl, string AppClientRolesUrl, string AppClientUuid)
    {
        public string GetUserUrl(string userId)
        {
            return $"{UsersUrl}/{Uri.EscapeDataString(userId)}";
        }

        public string GetRoleMappingsUrl(string userId)
        {
            return $"{GetUserUrl(userId)}/role-mappings/clients/{AppClientUuid}";
        }
    }
}
