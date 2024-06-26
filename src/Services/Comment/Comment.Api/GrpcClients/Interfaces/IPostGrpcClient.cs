using Shared.Dtos.Post;
using Shared.Dtos.Post.Queries;

namespace Comment.Api.GrpcClients.Interfaces;

public interface IPostGrpcClient
{
    Task<IEnumerable<PostDto>> GetTop10Posts();
}