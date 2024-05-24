using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Repositories.Interfaces;

public interface IRoleRepository
{
    #region CRUD

    Task CreateRole(IdentityRole model);

    Task<bool> UpdateRole(Guid roleId, IdentityRole model);

    Task<bool> DeleteRole(Guid roleId);

    Task<IEnumerable<IdentityRole>> GetRoles();

    Task<IdentityRole?> GetRoleById(Guid roleId);

    #endregion
}