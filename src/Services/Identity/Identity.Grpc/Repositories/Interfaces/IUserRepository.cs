using Identity.Grpc.Entities;

namespace Identity.Grpc.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserById(Guid userId);

    Task<User?> GetUserByUserName(string userName);
}