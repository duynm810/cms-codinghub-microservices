using Contracts.Domains.Repositories;
using Identity.Api.Entities;
using Shared.Dtos.Identity.Permission;

namespace Identity.Api.Repositories.Interfaces;

public interface IPermissionRepository : IRepositoryCommandBase<Permission, long>
{
    #region CRUD

    Task<PermissionDto?> CreatePermission(string roleId, CreatePermissionDto model);

    Task DeletePermission(string roleId, string function, string command);

    #endregion

    #region OTHERS

    Task<IReadOnlyList<PermissionDto>> GetPermissionsByRole(string roleId);
    
    Task UpdatePermissionsByRoleId(string roleId, IEnumerable<CreatePermissionDto> permissionCollection);

    Task<IEnumerable<PermissionUserDto>> GetPermissionsByUser(User user);

    #endregion
}