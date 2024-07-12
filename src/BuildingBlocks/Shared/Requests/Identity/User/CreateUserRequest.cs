namespace Shared.Requests.Identity.User;

public class CreateUserRequest : CreateOrUpdateUserRequest
{
    public required string UserName { get; set; }

    public required string Email { get; set; }

    public required string Password { get; set; }
}