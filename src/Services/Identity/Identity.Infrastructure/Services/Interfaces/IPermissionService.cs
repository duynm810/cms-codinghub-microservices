using Identity.Infrastructure.Entities;
using Shared.Dtos.Identity.Permission;
using Shared.Responses;

namespace Identity.Infrastructure.Services.Interfaces;

public interface IPermissionService
{
    #region CRUD

    Task<ApiResult<PermissionDto?>> CreatePermission(string roleId, CreateOrUpdatePermissionDto request);

    Task<ApiResult<bool>> UpdatePermissions(string roleId, IEnumerable<CreateOrUpdatePermissionDto> permissions);

    Task<ApiResult<bool>> DeletePermission(string roleId, string function, string command);

    Task<ApiResult<IReadOnlyList<PermissionDto>>> GetPermissions(string roleId);

    #endregion

    #region OTHERS

    Task<ApiResult<List<PermissionDto>>> GetPermissionsByUser(User user);

    #endregion
}