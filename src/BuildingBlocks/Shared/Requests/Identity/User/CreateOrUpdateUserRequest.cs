namespace Shared.Requests.Identity.User;

public class CreateOrUpdateUserRequest
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Address { get; set; }
    
    public string? About { get; set; }
}