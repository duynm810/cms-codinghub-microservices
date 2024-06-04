namespace Shared.Dtos.Identity.User;

public class CreateUserDto : CreateOrUpdateUserDto
{
    public required string UserName { get; set; }

    public required string Email { get; set; }

    public required string Password { get; set; }
}