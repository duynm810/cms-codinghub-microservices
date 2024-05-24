using Identity.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories;

public class RoleRepository(RoleManager<IdentityRole> roleManager) : IRoleRepository
{
    #region CRUD

    public async Task CreateRole(IdentityRole model) => await roleManager.CreateAsync(model);

    public async Task<bool> UpdateRole(Guid roleId, IdentityRole model)
    {
        var role = await roleManager.FindByIdAsync(roleId.ToString());
        if (role == null)
        {
            return false;
        }

        role.Name = model.Name;

        var result = await roleManager.UpdateAsync(role);
        return result.Succeeded;
    }

    public async Task<bool> DeleteRole(Guid roleId)
    {
        var role = await roleManager.FindByIdAsync(roleId.ToString());
        if (role == null)
        {
            return false;
        }

        var result = await roleManager.DeleteAsync(role);
        return result.Succeeded;
    }

    public async Task<IEnumerable<IdentityRole>> GetRoles() => await roleManager.Roles.ToListAsync();
    public async Task<IdentityRole?> GetRoleById(Guid roleId) => await roleManager.FindByIdAsync(roleId.ToString());

    #endregion
}