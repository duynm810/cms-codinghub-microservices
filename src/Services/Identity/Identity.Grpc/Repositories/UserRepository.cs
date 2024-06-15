using Identity.Grpc.Entities;
using Identity.Grpc.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Identity.Grpc.Repositories;

public class UserRepository(UserManager<User> userManager) : IUserRepository
{
    public async Task<User?> GetUserById(Guid userId) => await userManager.FindByIdAsync(userId.ToString());
}