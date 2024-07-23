using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Entities;

public class User : IdentityUser
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Address { get; set; }

    public string? AvatarUrl { get; set; }

    public string? About { get; set; }
}