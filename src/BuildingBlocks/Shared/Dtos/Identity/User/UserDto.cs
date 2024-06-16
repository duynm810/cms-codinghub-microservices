namespace Shared.Dtos.Identity.User;

public class UserDto
{
    public Guid UserId { get; set; }
    
    public string? FirstName { get; set; }

    public string? LastName { get; set; }
    
    public string FullName => $"{FirstName} {LastName}".Trim();

    public string? Address { get; set; }
}