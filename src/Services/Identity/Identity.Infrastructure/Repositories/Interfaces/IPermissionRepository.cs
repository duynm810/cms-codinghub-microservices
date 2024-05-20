using Contracts.Domains.Repositories;
using Identity.Infrastructure.Entities;
using Shared.Dtos.Identity.Permission;

namespace Identity.Infrastructure.Repositories.Interfaces;

public interface IPermissionRepository : IDapperRepositoryCommandBase<Permission, long>
{
    #region CRUD

    Task<PermissionDto?> CreatePermission(string roleId, CreateOrUpdatePermissionDto model);
    
    Task UpdatePermissions(string roleId, IEnumerable<CreateOrUpdatePermissionDto> permissions);
    
    Task DeletePermission(string roleId, string function, string command);
    
    Task<IReadOnlyList<PermissionDto>> GetPermissions(string roleId);

    #endregion
}