using Microsoft.AspNetCore.Identity;

public sealed class ApplicationRole : IdentityRole<int>
{
    public string RoleName { get; set; } = string.Empty;
}