using System.Data;
using AutoMapper;
using Identity.Infrastructure.Entities;
using Identity.Infrastructure.Repositories.Interfaces;
using Identity.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Serilog;
using Shared.Constants;
using Shared.Dtos.Identity.Permission;
using Shared.Responses;
using Shared.Utilities;

namespace Identity.Infrastructure.Services;

public class PermissionService(IIdentityReposityManager reposityManager, IMapper mapper, ILogger logger) : IPermissionService
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
            
            var permission = mapper.Map<Permission>(model);
            permission.RoleId = roleId;

            var executeResult = await reposityManager.Permissions.CreatePermission(roleId, permission);
            if (executeResult <= 0)
            {
                result.Messages.Add(ErrorMessagesConsts.Identity.Permission.PermissionCreationFailed);
                result.Failure(StatusCodes.Status400BadRequest, result.Messages);
                return result;
            }

            permission.Id = executeResult;
            
            var permissionDto = mapper.Map<PermissionDto>(permission);

            result.Success(permissionDto);

            logger.Information("END {MethodName} - Permission created successfully with ID {PermissionId}", methodName,
                permission.Id);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<bool>> UpdatePermissions(string roleId, IEnumerable<CreateOrUpdatePermissionDto> permissions)
    {
        var result = new ApiResult<bool>();
        const string methodName = nameof(UpdatePermissions);

        try
        {
            logger.Information("BEGIN {MethodName} - Updating permissions for role: {RoleId}", methodName, roleId);

            var permissionList = permissions.ToList();
            var updatePermissions = permissionList.Select(mapper.Map<Permission>).ToList();
            
            var dt = new DataTable();

            dt.Columns.Add("RoleId", typeof(string));
            dt.Columns.Add("Function", typeof(string));
            dt.Columns.Add("Command", typeof(string));

            foreach (var item in updatePermissions)
            {
                dt.Rows.Add(roleId, item.Function, item.Command);
            }

            await reposityManager.Permissions.UpdatePermissions(roleId, dt);

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

            await reposityManager.Permissions.DeletePermission(roleId, function, command);

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

            var queryResult = await reposityManager.Permissions.GetPermissions(roleId);

            var permissionDtos = mapper.Map<List<PermissionDto>>(queryResult);

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

    #region OTHERS

    public async Task<ApiResult<List<PermissionDto>>> GetPermissionsByUser(User user)
    {
        var result = new ApiResult<List<PermissionDto>>();
        const string methodName = nameof(GetPermissionsByUser);
        
        try
        {
            var permissions = await reposityManager.Permissions.GetPermissionsByUser(user);
            var data = mapper.Map<List<PermissionDto>>(permissions);
            result.Success(data);
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