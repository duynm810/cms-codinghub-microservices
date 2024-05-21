using System.Data;
using Contracts.Domains.Repositories;
using Dapper;
using Identity.Infrastructure.Entities;
using Shared.Dtos.Identity.Permission;

namespace Identity.Infrastructure.Repositories.Interfaces;

public interface IPermissionRepository : IRepositoryCommandBase<Permission, long>
{
    #region CRUD

    Task<long> CreatePermission(string roleId, Permission model);

    Task<long> UpdatePermissions(string roleId, DataTable permissions);

    Task<long> DeletePermission(string roleId, string function, string command);

    Task<IEnumerable<Permission>> GetPermissions(string roleId);

    #endregion
}