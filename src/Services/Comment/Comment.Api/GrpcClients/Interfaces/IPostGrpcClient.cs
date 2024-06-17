using Shared.Dtos.Post;

namespace Comment.Api.GrpcClients.Interfaces;

public interface IPostGrpcClient
{
    Task<IEnumerable<PostDto>?> GetTop10Posts();
}