namespace Shared.Dtos.Identity.User;

public class CurrentUserDto
{
    public Guid UserId { get; set; }

    public List<string> Roles { get; set; } = [];
}