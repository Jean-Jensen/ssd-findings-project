namespace Application.Models.Constants;

public static class RoleConstants
{
    public const string Analyst = "Analyst";
    public const string Developer = "Developer";
    public const string PM = "PM";
    public const string Auditor = "Auditor";

    public static bool IsAppRole(string role) => role is Analyst or Developer or PM or Auditor;
}
