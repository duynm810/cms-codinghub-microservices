using Shared.Dtos.Identity.User;

namespace Comment.Api.GrpcClients.Interfaces;

public interface IIdentityGrpcClient
{
    Task<UserDto?> GetUserInfo(string userId);
}