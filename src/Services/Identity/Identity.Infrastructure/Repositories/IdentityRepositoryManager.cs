using AutoMapper;
using Contracts.Domains.Repositories;
using Identity.Infrastructure.Entities;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Identity.Infrastructure.Repositories;

public class IdentityRepositoryManager(
    IdentityContext dbContext,
    IUnitOfWork<IdentityContext> unitOfWork,
    UserManager<User> userManager,
    RoleManager<IdentityRole> roleManager,
    IMapper mapper,
    ILogger logger) : IIdentityReposityManager
{
    private readonly Lazy<IPermissionRepository> _permissionRepository =
        new(() => new PermissionRepository(dbContext, unitOfWork, userManager, mapper, logger));

    public IPermissionRepository Permissions => _permissionRepository.Value;

    public UserManager<User> UserManager { get; } = userManager;

    public RoleManager<IdentityRole> RoleManager { get; } = roleManager;
}