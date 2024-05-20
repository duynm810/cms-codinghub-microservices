using Contracts.Domains.Repositories;
using Identity.Infrastructure.Entities;
using Shared.Dtos.Identity.Permission;

namespace Identity.Infrastructure.Repositories.Interfaces;

public interface IPermissionRepository : IDapperRepositoryCommandBase<Permission, long>
{
    #region CRUD

    Task<PermissionDto?> CreatePermission(string roleId, CreatePermissionDto model);

    #endregion
}