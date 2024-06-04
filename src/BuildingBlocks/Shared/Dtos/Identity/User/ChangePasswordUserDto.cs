namespace Shared.Dtos.Identity.User;

public class ChangePasswordUserDto
{
    public required string CurrentPassword { get; set; }

    public required string NewPassword { get; set; }
}