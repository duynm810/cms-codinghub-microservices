using Shared.Dtos.Tag;

namespace Post.Domain.Services;

public interface IPostEventService
{
    Task HandlePostCreatedEvent(Guid postId, List<RawTagDto> rawTags);
}