using Identity.Infrastructure.Services.Interfaces;
using Shared.Dtos.Identity.Role;
using Shared.Responses;

namespace Identity.Infrastructure.Services;

public class RoleService : IRoleService
{
    #region CRUD

    public async Task<ApiResult<RoleDto?>> CreateRole(CreateOrUpdateRoleDto model)
    {
        throw new NotImplementedException();
    }

    public async Task<ApiResult<bool>> UpdateRole(Guid roleId, CreateOrUpdateRoleDto model)
    {
        throw new NotImplementedException();
    }

    public async Task<ApiResult<bool>> DeleteRole(Guid roleId)
    {
        throw new NotImplementedException();
    }

    public async Task<ApiResult<IReadOnlyList<RoleDto>>> GetRoles()
    {
        throw new NotImplementedException();
    }

    public async Task<ApiResult<RoleDto>> GetRoleById(Guid roleId)
    {
        throw new NotImplementedException();
    }

    #endregion
}