using Identity.Infrastructure.Entities;
using Identity.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories;

public class UserRepository(UserManager<User> userManager) : IUserRepository
{
    #region CRUD

    public async Task CreateUser(User user, string password)
    {
        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
        }
    }

    public async Task<bool> UpdateUser(User user)
    {
        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        return result.Succeeded;
    }

    public async Task<bool> DeleteUser(User user)
    {
        var result = await userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    public async Task<IEnumerable<User>> GetUsers() => await userManager.Users.ToListAsync();

    public async Task<User?> GetUserById(Guid userId) => await userManager.FindByIdAsync(userId.ToString());

    #endregion

    #region OTHERS

    public async Task<User?> GetUserByUserName(string userName) => await userManager.FindByNameAsync(userName);

    public async Task<bool> ChangePassword(User user, string currentPassword, string newPassword)
    {
        var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!result.Succeeded)
        {
            throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        return result.Succeeded;
    }

    #endregion
}