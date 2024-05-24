using Identity.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Serilog;
using Shared.Dtos.Identity.Role;
using Shared.Responses;
using Shared.Utilities;

namespace Identity.Infrastructure.Services;

public class RoleService(ILogger logger) : IRoleService
{
    #region CRUD

    public async Task<ApiResult<RoleDto?>> CreateRole(CreateOrUpdateRoleDto model)
    {
        var result = new ApiResult<RoleDto?>();
        const string methodName = nameof(CreateRole);

        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<bool>> UpdateRole(Guid roleId, CreateOrUpdateRoleDto model)
    {
        var result = new ApiResult<bool>();
        const string methodName = nameof(UpdateRole);

        try
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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