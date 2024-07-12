namespace Shared.Requests.Identity.Permission;

public class CreateOrUpdatePermissionRequest
{
    public required string Function { get; set; }

    public required string Command { get; set; }
}