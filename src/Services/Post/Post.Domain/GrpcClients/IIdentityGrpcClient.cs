using Shared.Dtos.Identity.User;

namespace Post.Domain.GrpcClients;

public interface IIdentityGrpcClient
{
    Task<UserDto?> GetUserInfo(Guid userId);
    
    Task<UserDto?> GetUserInfoByUserName(string userName);
}