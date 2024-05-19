using Identity.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;

namespace Identity.Api.Repositories.Interfaces;

public interface IIdentityReposityManager
{
    UserManager<User> UserManager { get; }

    RoleManager<IdentityRole> RoleManager { get; }

    #region FUNCTIONS

    Task<int> SaveAsync();

    Task<IDbContextTransaction> BeginTransactionAsync();

    Task EndTransactionAsync();

    void RollbackTransaction();

    #endregion
}