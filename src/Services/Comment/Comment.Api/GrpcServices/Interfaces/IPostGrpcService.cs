using Shared.Dtos.Post;

namespace Comment.Api.GrpcServices.Interfaces;

public interface IPostGrpcService
{
    Task<IEnumerable<PostDto>> GetTop10Posts();
}