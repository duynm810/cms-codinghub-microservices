using Identity.Infrastructure.Entities;

namespace Identity.Infrastructure.Repositories.Interfaces;

public interface IUserRepository
{
    #region CRUD

    Task CreateUser(User user, string password);

    Task<bool> UpdateUser(User user);

    Task<bool> DeleteUser(User user);

    Task<IEnumerable<User>> GetUsers();

    Task<User?> GetUserById(Guid userId);

    #endregion

    #region OTHERS

    Task<User?> GetUserByUserName(string userName);

    Task<bool> ChangePassword(User user, string currentPassword, string newPassword);

    #endregion
}