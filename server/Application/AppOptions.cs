using System.ComponentModel.DataAnnotations;

namespace Application;

public sealed class AppOptions
{
    [Required] public string DbConnectionString { get; set; } = string.Empty!;
    public bool Seed { get; set; } = true;
    public int REST_PORT { get; set; } = 5001;
}
