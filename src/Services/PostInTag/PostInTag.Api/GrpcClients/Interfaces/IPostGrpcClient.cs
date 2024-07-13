using Shared.Dtos.Post;
using Shared.Dtos.PostInTag;

namespace PostInTag.Api.GrpcClients.Interfaces;

public interface IPostGrpcClient
{
    Task<IEnumerable<PostInTagDto>> GetPostsByIds(IEnumerable<Guid> ids);

    Task<IEnumerable<PostDto>> GetTop10Posts();
}