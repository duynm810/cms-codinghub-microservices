using Shared.Dtos.Tag;

namespace Post.Domain.GrpcServices;

public interface ITagGrpcService
{
    Task<IEnumerable<TagDto>> GetTagsByIds(IEnumerable<Guid> ids);
    
    Task<IEnumerable<TagDto>> GetTags();
}