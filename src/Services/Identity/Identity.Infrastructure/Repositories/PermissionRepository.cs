using System.Data;
using AutoMapper;
using Contracts.Domains.Repositories;
using Dapper;
using Identity.Infrastructure.Entities;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Repositories.Interfaces;
using Infrastructure.Domains.Repositories;
using Microsoft.AspNetCore.Identity;
using Shared.Dtos.Identity.Permission;

namespace Identity.Infrastructure.Repositories;

public class PermissionRepository(
    IdentityContext dbContext,
    IUnitOfWork<IdentityContext> unitOfWork,
    UserManager<User> userManager,
    IMapper mapper)
    : DapperRepositoryBase<Permission, long>(dbContext), IPermissionRepository
{
    #region CRUD

    public async Task<PermissionDto?> CreatePermission(string roleId, CreatePermissionDto model)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@roleId", roleId, DbType.String);
        parameters.Add("@function", model.Function, DbType.String);
        parameters.Add("@command", model.Command, DbType.String);
        parameters.Add("@newID", dbType: DbType.Int64, direction: ParameterDirection.Output);

        var result = await ExecuteAsync("Create_Permission", parameters);
        if (result <= 0) return null;
        var newId = parameters.Get<long>("@newID");
        return new PermissionDto
        {
            Id = newId,
            RoleId = roleId,
            Function = model.Function,
            Command = model.Command
        };
    }

    #endregion
}