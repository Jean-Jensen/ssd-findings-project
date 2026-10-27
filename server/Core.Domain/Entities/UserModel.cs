namespace Core.Domain.Entities;

public class UserModel
{
    public bool IsAuthenticated { get; set; }
    public string? Name { get; set; }
    public string? EmailAddress { get; set; }
    public Dictionary<string, string> AdditionalClaims { get; set; } = new();
}