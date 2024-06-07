using Shared.Dtos.Post;
using Shared.Dtos.PostInTag;

namespace PostInTag.Api.GrpcServices.Interfaces;

public interface IPostGrpcService
{
    Task<IEnumerable<PostInTagDto>> GetPostsByIds(IEnumerable<Guid> ids);

    Task<IEnumerable<PostDto>> GetTop10Posts();
}