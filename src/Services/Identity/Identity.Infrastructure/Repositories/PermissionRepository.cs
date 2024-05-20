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
    : DapperRepositoryBase<Permission, long>(dbContext), IPermissionRepository
{
    #region CRUD

    public async Task<PermissionDto?> CreatePermission(string roleId, CreateOrUpdatePermissionDto model)
    {
        const string methodName = nameof(CreatePermission);
        try
        {
            logger.Information(
                "BEGIN {MethodName} - Creating permission for role: {RoleId} with function: {Function} and command: {Command}",
                methodName, roleId, model.Function, model.Command);

            var parameters = new DynamicParameters();
            parameters.Add("@roleId", roleId, DbType.String);
            parameters.Add("@function", model.Function, DbType.String);
            parameters.Add("@command", model.Command, DbType.String);
            parameters.Add("@newID", dbType: DbType.Int64, direction: ParameterDirection.Output);

            var result = await ExecuteAsync("Create_Permission", parameters);
            if (result <= 0)
            {
                logger.Warning("END {MethodName} - Failed to create permission for role: {RoleId}", methodName, roleId);
                return null;
            }

            var newId = parameters.Get<long>("@newID");
            var permission = new PermissionDto
            {
                Id = newId,
                RoleId = roleId,
                Function = model.Function,
                Command = model.Command
            };

            logger.Information("END {MethodName} - Permission created successfully with ID {PermissionId}", methodName,
                newId);
            return permission;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e.Message);
            throw;
        }
    }

    public async Task UpdatePermissions(string roleId, IEnumerable<CreateOrUpdatePermissionDto> permissions)
    {
        const string methodName = nameof(UpdatePermissions);
        try
        {
            logger.Information("BEGIN {MethodName} - Updating permissions for role: {RoleId}", methodName, roleId);

            var dt = new DataTable();
            dt.Columns.Add("RoleId", typeof(string));
            dt.Columns.Add("Function", typeof(string));
            dt.Columns.Add("Command", typeof(string));
            foreach (var item in permissions)
            {
                dt.Rows.Add(roleId, item.Function, item.Command);
            }

            var parameters = new DynamicParameters();
            parameters.Add("@roleId", roleId, DbType.String);
            parameters.Add("@permissions", dt.AsTableValuedParameter("dbo.Permission"));
            await ExecuteAsync("Update_Permissions", parameters);

            logger.Information("END {MethodName} - Permissions updated successfully for role: {RoleId}", methodName,
                roleId);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e.Message);
            throw;
        }
    }

    public async Task DeletePermission(string roleId, string function, string command)
    {
        const string methodName = nameof(DeletePermission);
        try
        {
            logger.Information(
                "BEGIN {MethodName} - Deleting permission for role: {RoleId} with function: {Function} and command: {Command}",
                methodName, roleId, function, command);

            var parameters = new DynamicParameters();
            parameters.Add("@roleId", roleId, DbType.String);
            parameters.Add("@function", function, DbType.String);
            parameters.Add("@command", command, DbType.String);

            await ExecuteAsync("Delete_Permission", parameters);

            logger.Information("END {MethodName} - Permission deleted successfully for role: {RoleId}", methodName,
                roleId);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e.Message);
            throw;
        }
    }

    public async Task<IReadOnlyList<PermissionDto>> GetPermissions(string roleId)
    {
        const string methodName = nameof(GetPermissions);
        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving permissions for role: {RoleId}", methodName, roleId);

            var parameters = new DynamicParameters();
            parameters.Add("@roleId", roleId);

            var result = await QueryAsync<PermissionDto>("Get_Permissions", parameters);

            logger.Information(
                "END {MethodName} - Successfully retrieved {PermissionCount} permissions for role: {RoleId}",
                methodName, result.Count, roleId);
            return result;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e.Message);
            throw;
        }
    }

    #endregion
}