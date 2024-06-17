using Shared.Dtos.Identity.User;

namespace Comment.Api.GrpcClients.Interfaces;

public interface IIdentityGrpcClient
{
    Task<List<UserDto>> GetUsersInfo(IEnumerable<Guid> userIds);
}