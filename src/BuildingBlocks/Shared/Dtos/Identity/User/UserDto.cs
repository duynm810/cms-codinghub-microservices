namespace Shared.Dtos.Identity.User;

public class UserDto
{
    public Guid Id { get; set; }
    
    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;
    
    public string UserName { get; set; } = default!;
    
    public string FullName => $"{FirstName} {LastName}".Trim();

    public string Address { get; set; } = default!;
}