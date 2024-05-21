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

    public IPermissionRepository Permissions => _permissionRepository.Value;

    public UserManager<User> UserManager { get; } = userManager;

    public RoleManager<IdentityRole> RoleManager { get; } = roleManager;
}