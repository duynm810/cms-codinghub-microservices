using Shared.Dtos.Tag;

namespace PostInTag.Api.GrpcServices.Interfaces;

public interface ITagGrpcService
{
    Task<TagDto?> GetTagBySlug(string slug);

    Task<IEnumerable<TagDto>> GetTags();
}