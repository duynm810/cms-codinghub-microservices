using Shared.Dtos.Post.Queries;

namespace Comment.Api.GrpcClients.Interfaces;

public interface IPostGrpcClient
{
    Task<List<PostDto>> GetTop10Posts();

    Task<List<PostDto>> GetPostsByIds(IEnumerable<Guid> ids);
}