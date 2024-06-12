using Shared.Dtos.Tag;

namespace PostInTag.Api.GrpcClients.Interfaces;

public interface ITagGrpcClient
{
    Task<TagDto?> GetTagBySlug(string slug);

    Task<IEnumerable<TagDto>> GetTags();
}