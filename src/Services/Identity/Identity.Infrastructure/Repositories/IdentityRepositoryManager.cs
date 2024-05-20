using AutoMapper;
using Contracts.Domains.Repositories;
using Identity.Infrastructure.Entities;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;

namespace Identity.Infrastructure.Repositories;

public class IdentityRepositoryManager(IdentityContext dbContext, IUnitOfWork<IdentityContext> unitOfWork, IMapper mapper, UserManager<User> userManager, RoleManager<IdentityRole> roleManager) : IIdentityReposityManager
{
    private readonly Lazy<IPermissionRepository> _permissionRepository =
        new(() => new PermissionRepository(dbContext, unitOfWork, userManager, mapper));

    public UserManager<User> UserManager { get; } = userManager;

    public RoleManager<IdentityRole> RoleManager { get; } = roleManager;

    public IPermissionRepository Permissions => _permissionRepository.Value;

    #region FUNCTIONS

    public Task<int> SaveAsync() => unitOfWork.CommitAsync();

    public Task<IDbContextTransaction> BeginTransactionAsync() => dbContext.Database.BeginTransactionAsync();

    public Task EndTransactionAsync() => dbContext.Database.CommitTransactionAsync();

    public void RollbackTransaction() => dbContext.Database.RollbackTransactionAsync();

    #endregion
}