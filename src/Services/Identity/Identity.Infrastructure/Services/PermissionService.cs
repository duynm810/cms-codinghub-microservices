using System.Data;
using Dapper;
using Identity.Infrastructure.Repositories.Interfaces;
using Identity.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Serilog;
using Shared.Constants;
using Shared.Dtos.Identity.Permission;
using Shared.Responses;
using Shared.Utilities;

namespace Identity.Infrastructure.Services;

public class PermissionService(IPermissionRepository permissionRepository, ILogger logger) : IPermissionService
{
    #region CRUD

    public async Task<ApiResult<PermissionDto?>> CreatePermission(string roleId, CreateOrUpdatePermissionDto model)
    {
        var result = new ApiResult<PermissionDto?>();
        const string methodName = nameof(CreatePermission);

        try
        {
            logger.Information(
                "BEGIN {MethodName} - Creating permission for role: {RoleId} with function: {Function} and command: {Command}",
                methodName, roleId, model.Function, model.Command);

            var parameters = new DynamicParameters();
            var executeResult = await permissionRepository.CreatePermission(roleId, model, parameters);
            if (executeResult <= 0)
            {
                result.Messages.Add(ErrorMessageConsts.Identity.Permission.PermissionCreationFailed);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var newId = parameters.Get<long>("@newID");
            var permission = new PermissionDto
            {
                Id = newId,
                RoleId = roleId,
                Function = model.Function,
                Command = model.Command
            };

            result.Success(permission);

            logger.Information("END {MethodName} - Permission created successfully with ID {PermissionId}", methodName,
                newId);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<bool>> UpdatePermissions(string roleId,
        IEnumerable<CreateOrUpdatePermissionDto> permissions)
    {
        var result = new ApiResult<bool>();
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

            await permissionRepository.UpdatePermissions(roleId, dt);

            result.Success(true);

            logger.Information("END {MethodName} - Permissions updated successfully for role: {RoleId}", methodName,
                roleId);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<bool>> DeletePermission(string roleId, string function, string command)
    {
        var result = new ApiResult<bool>();
        const string methodName = nameof(DeletePermission);

        try
        {
            logger.Information(
                "BEGIN {MethodName} - Deleting permission for role: {RoleId} with function: {Function} and command: {Command}",
                methodName, roleId, function, command);

            await permissionRepository.DeletePermission(roleId, function, command);

            result.Success(true);

            logger.Information("END {MethodName} - Permission deleted successfully for role: {RoleId}", methodName,
                roleId);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<IReadOnlyList<PermissionDto>>> GetPermissions(string roleId)
    {
        var result = new ApiResult<IReadOnlyList<PermissionDto>>();
        const string methodName = nameof(GetPermissions);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving permissions for role: {RoleId}", methodName, roleId);

            var queryResult = await permissionRepository.GetPermissions(roleId);

            var permissionDtos = queryResult.ToList();

            result.Success(permissionDtos.ToList());

            logger.Information(
                "END {MethodName} - Successfully retrieved {PermissionCount} permissions for role: {RoleId}",
                methodName, permissionDtos.Count, roleId);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    #endregion
}