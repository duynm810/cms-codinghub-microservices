using System.Data;
using AutoMapper;
using Contracts.Domains.Repositories;
using Dapper;
using Identity.Infrastructure.Entities;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Repositories.Interfaces;
using Infrastructure.Domains.Repositories;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Shared.Dtos.Identity.Permission;

namespace Identity.Infrastructure.Repositories;

public class PermissionRepository(
    IdentityContext dbContext,
    IUnitOfWork<IdentityContext> unitOfWork,
    UserManager<User> userManager,
    IMapper mapper,
    ILogger logger)
    : RepositoryCommandBase<Permission, long, IdentityContext>(dbContext, unitOfWork), IPermissionRepository
{
    #region CRUD

    public async Task<int> CreatePermission(string roleId, CreateOrUpdatePermissionDto model,
        DynamicParameters parameters)
    {
        parameters.Add("@roleId", roleId, DbType.String);
        parameters.Add("@function", model.Function, DbType.String);
        parameters.Add("@command", model.Command, DbType.String);
        parameters.Add("@newID", dbType: DbType.Int64, direction: ParameterDirection.Output);

        return await ExecuteAsync("Create_Permission", parameters);
    }

    public async Task<int> UpdatePermissions(string roleId, DataTable permissions)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@roleId", roleId, DbType.String);
        parameters.Add("@permissions", permissions.AsTableValuedParameter("dbo.Permission"));

        return await ExecuteAsync("Update_Permissions", parameters);
    }

    public async Task<int> DeletePermission(string roleId, string function, string command)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@roleId", roleId, DbType.String);
        parameters.Add("@function", function, DbType.String);
        parameters.Add("@command", command, DbType.String);

        return await ExecuteAsync("Delete_Permission", parameters);
    }

    public async Task<IEnumerable<PermissionDto>> GetPermissions(string roleId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@roleId", roleId);

        return await QueryAsync<PermissionDto>("Get_Permissions", parameters);
    }

    #endregion
}