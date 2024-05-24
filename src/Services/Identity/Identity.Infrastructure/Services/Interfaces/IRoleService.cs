using Shared.Dtos.Identity.Role;
using Shared.Responses;

namespace Identity.Infrastructure.Services.Interfaces;

public interface IRoleService
{
    #region CRUD

    Task<ApiResult<RoleDto?>> CreateRole(CreateOrUpdateRoleDto model);

    Task<ApiResult<bool>> UpdateRole(Guid roleId, CreateOrUpdateRoleDto model);

    Task<ApiResult<bool>> DeleteRole(Guid roleId);

    Task<ApiResult<IEnumerable<RoleDto>>> GetRoles();

    Task<ApiResult<RoleDto>> GetRoleById(Guid roleId);
    
    #endregion
}