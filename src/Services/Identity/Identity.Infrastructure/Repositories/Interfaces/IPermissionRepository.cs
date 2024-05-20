using Contracts.Domains.Repositories;
using Identity.Infrastructure.Entities;
using Shared.Dtos.Identity.Permission;

namespace Identity.Infrastructure.Repositories.Interfaces;

public interface IPermissionRepository : IRepositoryCommandBase<Permission, long>
{
    #region CRUD

    Task<PermissionDto?> CreatePermission(string roleId, CreatePermissionDto model);

    Task UpdatePermission(string roleId, IEnumerable<CreatePermissionDto> permissionCollection);

    Task DeletePermission(string roleId, string function, string command);

    #endregion

    #region OTHERS

    Task<IReadOnlyList<PermissionDto>> GetPermissionsByRole(string roleId);
    
    Task<IEnumerable<PermissionUserDto>> GetPermissionsByUser(User user);

    #endregion
}