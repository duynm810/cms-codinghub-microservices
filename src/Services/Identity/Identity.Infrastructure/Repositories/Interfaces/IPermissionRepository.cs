using System.Data;
using Contracts.Domains.Repositories;
using Dapper;
using Identity.Infrastructure.Entities;
using Shared.Dtos.Identity.Permission;

namespace Identity.Infrastructure.Repositories.Interfaces;

public interface IPermissionRepository : IDapperRepositoryCommandBase<Permission, long>
{
    #region CRUD

    Task<int> CreatePermission(string roleId, CreateOrUpdatePermissionDto model, DynamicParameters parameters);

    Task<int> UpdatePermissions(string roleId, DataTable permissions);

    Task<int> DeletePermission(string roleId, string function, string command);

    Task<IEnumerable<PermissionDto>> GetPermissions(string roleId);

    #endregion
}