namespace Shared.Requests.Identity.User;

public class ChangePasswordUserRequest
{
    public required string CurrentPassword { get; set; }

    public required string NewPassword { get; set; }
}