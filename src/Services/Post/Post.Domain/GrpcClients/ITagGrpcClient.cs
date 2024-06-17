using Shared.Dtos.Tag;

namespace Post.Domain.GrpcClients;

public interface ITagGrpcClient
{
    Task<IEnumerable<TagDto>?> GetTagsByIds(IEnumerable<Guid> ids);
}