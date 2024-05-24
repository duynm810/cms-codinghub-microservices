using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Repositories.Interfaces;

public interface IRoleRepository
{
    #region CRUD

    Task CreateRole(IdentityRole role);

    Task<bool> UpdateRole(Guid roleId, IdentityRole role);

    Task<bool> DeleteRole(IdentityRole role);

    Task<IEnumerable<IdentityRole>> GetRoles();

    Task<IdentityRole?> GetRoleById(Guid roleId);

    #endregion
}