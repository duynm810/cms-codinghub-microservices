using AutoMapper;
using Contracts.Domains.Repositories;
using Identity.Api.Entities;
using Identity.Api.Persistence;
using Identity.Api.Repositories.Interfaces;
using Infrastructure.Domains.Repositories;
using Microsoft.AspNetCore.Identity;
using Shared.Dtos.Identity.Permission;

namespace Identity.Api.Repositories;

public class PermissionRepository(IdentityContext dbContext, IUnitOfWork<IdentityContext> unitOfWork, UserManager<User> userManager, IMapper mapper)
    : RepositoryCommandBase<Permission, long, IdentityContext>(dbContext, unitOfWork), IPermissionRepository
{
    #region CRUD

    public async Task<PermissionDto?> CreatePermission(string roleId, CreatePermissionDto model)
    {
        throw new NotImplementedException();
    }

    public async Task DeletePermission(string roleId, string function, string command)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region OTHERS

    public async Task<IReadOnlyList<PermissionDto>> GetPermissionsByRole(string roleId)
    {
        throw new NotImplementedException();
    }

    public async Task UpdatePermissionsByRoleId(string roleId, IEnumerable<CreatePermissionDto> permissionCollection)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<PermissionUserDto>> GetPermissionsByUser(User user)
    {
        throw new NotImplementedException();
    }

    #endregion
}