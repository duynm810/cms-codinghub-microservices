using AutoMapper;
using Contracts.Domains.Repositories;
using Identity.Api.Entities;
using Identity.Api.Persistence;
using Identity.Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;

namespace Identity.Api.Repositories;

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