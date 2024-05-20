using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.Identity.Permission;

public class CreateOrUpdatePermissionDto
{
    public required string Function { get; set; }

    public required string Command { get; set; }
}