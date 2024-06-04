namespace Shared.Dtos.Identity.Permission;

public class PermissionDto
{
    public long Id { get; set; }

    public string Function { get; set; } = default!;

    public string Command { get; set; } = default!;

    public string RoleId { get; set; } = default!;
}