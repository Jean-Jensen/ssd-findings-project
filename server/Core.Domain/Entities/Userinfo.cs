using System;
using System.Collections.Generic;

namespace Core.Domain.Entities;

public partial class Userinfo
{
    public bool? Isauthenticated { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Additionalclaims { get; set; }
}
