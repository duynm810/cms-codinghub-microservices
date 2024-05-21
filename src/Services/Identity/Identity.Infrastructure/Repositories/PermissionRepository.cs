using System.Data;
using Contracts.Domains.Repositories;
using Dapper;
using Identity.Infrastructure.Entities;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Repositories.Interfaces;
using Infrastructure.Domains.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;

namespace Identity.Infrastructure.Repositories;

public class PermissionRepository(
    IdentityContext dbContext,
    IUnitOfWork<IdentityContext> unitOfWork,
    UserManager<User> userManager)
    : RepositoryCommandBase<Permission, long, IdentityContext>(dbContext, unitOfWork), IPermissionRepository
{
    #region CRUD

    public async Task<long> CreatePermission(string roleId, Permission model)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@roleId", roleId, DbType.String);
        parameters.Add("@function", model.Function, DbType.String);
        parameters.Add("@command", model.Command, DbType.String);
        parameters.Add("@newID", dbType: DbType.Int64, direction: ParameterDirection.Output);

        await ExecuteAsync(StoredProceduresConsts.Permission.CreatePermission, parameters);

        var newId = parameters.Get<long>("@newID");
        return newId;
    }

    public async Task<long> UpdatePermissions(string roleId, DataTable permissions)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@roleId", roleId, DbType.String);
        parameters.Add("@permissions", permissions.AsTableValuedParameter("dbo.Permission"));

        return await ExecuteAsync(StoredProceduresConsts.Permission.UpdatePermissions, parameters);
    }

    public async Task<long> DeletePermission(string roleId, string function, string command)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@roleId", roleId, DbType.String);
        parameters.Add("@function", function, DbType.String);
        parameters.Add("@command", command, DbType.String);

        return await ExecuteAsync(StoredProceduresConsts.Permission.DeletePermission, parameters);
    }

    public async Task<IEnumerable<Permission>> GetPermissions(string roleId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@roleId", roleId);

        return await QueryAsync<Permission>(StoredProceduresConsts.Permission.GetPermissions, parameters);
    }

    #endregion

    #region OTHERS

    public async Task<IEnumerable<Permission>> GetPermissionsByUser(User user)
    {
        var currentUserRoles = await userManager.GetRolesAsync(user);
        var query = await FindByCondition(x => currentUserRoles.Contains(x.RoleId)).ToListAsync();
        return query;
    }

    #endregion
}