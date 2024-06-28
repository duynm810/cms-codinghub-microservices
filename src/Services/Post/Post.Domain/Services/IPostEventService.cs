using EventBus.IntegrationEvents.Posts;

namespace Post.Domain.Services;

public interface IPostEventService
{
    Task HandlePostCreatedEvent(PostCreatedEvent postCreatedEvent);
}