using Contracts.Domains.Repositories;
using Identity.Infrastructure.Entities;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Repositories;

public class IdentityRepositoryManager(
    IdentityContext dbContext,
    IUnitOfWork<IdentityContext> unitOfWork,
    UserManager<User> userManager,
    RoleManager<IdentityRole> roleManager) : IIdentityReposityManager
{
    private readonly Lazy<IPermissionRepository> _permissionRepository =
        new(() => new PermissionRepository(dbContext, unitOfWork, userManager));

    private readonly Lazy<IRoleRepository> _roleRepository = new(() => new RoleRepository(roleManager));

    private readonly Lazy<IUserRepository> _userRepository = new(() => new UserRepository(userManager));

    public IPermissionRepository Permissions => _permissionRepository.Value;
    
    public IRoleRepository Roles => _roleRepository.Value;
    
    public IUserRepository Users => _userRepository.Value;

    public UserManager<User> UserManager { get; } = userManager;

    public RoleManager<IdentityRole> RoleManager { get; } = roleManager;
}