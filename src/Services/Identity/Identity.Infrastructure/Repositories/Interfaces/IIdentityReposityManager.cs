using Identity.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;

namespace Identity.Infrastructure.Repositories.Interfaces;

public interface IIdentityReposityManager
{
    UserManager<User> UserManager { get; }

    RoleManager<IdentityRole> RoleManager { get; }
    
    IPermissionRepository Permissions { get; }
    
    IRoleRepository Roles { get; }
}