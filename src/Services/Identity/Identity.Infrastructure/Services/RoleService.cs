using AutoMapper;
using Identity.Infrastructure.Repositories.Interfaces;
using Identity.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Shared.Constants;
using Shared.Dtos.Identity.Role;
using Shared.Requests.Identity.Role;
using Shared.Responses;
using Shared.Utilities;

namespace Identity.Infrastructure.Services;

public class RoleService(IIdentityReposityManager repositoryManager, IMapper mapper, ILogger logger) : IRoleService
{
    #region CRUD

    public async Task<ApiResult<RoleDto?>> CreateRole(CreateOrUpdateRoleRequest request)
    {
        var result = new ApiResult<RoleDto?>();
        const string methodName = nameof(CreateRole);

        try
        {
            logger.Information("BEGIN {MethodName} - Creating role with name: {RoleName}", methodName, request.Name);

            var role = mapper.Map<IdentityRole>(request);
            await repositoryManager.Roles.CreateRole(role);

            var data = mapper.Map<RoleDto>(role);
            result.Success(data);

            logger.Information("END {MethodName} - Role created successfully with ID {RoleId}", methodName, role.Id);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<bool>> UpdateRole(Guid roleId, CreateOrUpdateRoleRequest request)
    {
        var result = new ApiResult<bool>();
        const string methodName = nameof(UpdateRole);

        try
        {
            logger.Information("BEGIN {MethodName} - Updating role with ID: {RoleId}", methodName, roleId);

            var role = await repositoryManager.Roles.GetRoleById(roleId);
            if (role == null)
            {
                result.Messages.Add(ErrorMessagesConsts.Identity.Role.RoleNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            role.Name = request.Name;

            var updateResult = await repositoryManager.Roles.UpdateRole(roleId, role);
            if (!updateResult)
            {
                result.Messages.Add(ErrorMessagesConsts.Identity.Role.RoleUpdateFailed);
                result.Failure(StatusCodes.Status400BadRequest, result.Messages);
                return result;
            }

            result.Success(updateResult);

            logger.Information("END {MethodName} - Role updated successfully with ID {RoleId}", methodName, roleId);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<bool>> DeleteRole(Guid roleId)
    {
        var result = new ApiResult<bool>();
        const string methodName = nameof(DeleteRole);

        try
        {
            logger.Information("BEGIN {MethodName} - Deleting role with ID: {RoleId}", methodName, roleId);

            var role = await repositoryManager.Roles.GetRoleById(roleId);
            if (role == null)
            {
                result.Messages.Add(ErrorMessagesConsts.Identity.Role.RoleNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var deleteResult = await repositoryManager.Roles.DeleteRole(role);
            if (!deleteResult)
            {
                result.Messages.Add(ErrorMessagesConsts.Identity.Role.RoleDeleteFailed);
                result.Failure(StatusCodes.Status400BadRequest, result.Messages);
                return result;
            }

            result.Success(deleteResult);

            logger.Information("END {MethodName} - Role deleted successfully with ID {RoleId}", methodName, roleId);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<IEnumerable<RoleDto>>> GetRoles()
    {
        var result = new ApiResult<IEnumerable<RoleDto>>();
        const string methodName = nameof(GetRoles);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving all roles", methodName);

            var roles = await repositoryManager.Roles.GetRoles();
            var data = mapper.Map<IEnumerable<RoleDto>>(roles);

            result.Success(data);

            logger.Information("END {MethodName} - Successfully retrieved roles", methodName);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<RoleDto>> GetRoleById(Guid roleId)
    {
        var result = new ApiResult<RoleDto>();
        const string methodName = nameof(GetRoleById);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving role with ID: {RoleId}", methodName, roleId);

            var role = await repositoryManager.Roles.GetRoleById(roleId);
            if (role == null)
            {
                result.Messages.Add(ErrorMessagesConsts.Identity.Role.RoleNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var data = mapper.Map<RoleDto>(role);
            result.Success(data);

            logger.Information("END {MethodName} - Successfully retrieved role with ID {RoleId}", methodName, roleId);
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