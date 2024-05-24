using Identity.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    #region CRUD

    public async Task CreateRole(IdentityRole model)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateRole(Guid roleId, IdentityRole model)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteRole(Guid roleId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<IdentityRole>> GetRoles()
    {
        throw new NotImplementedException();
    }

    public async Task<IdentityRole> GetRoleById(Guid roleId)
    {
        throw new NotImplementedException();
    }

    #endregion
}