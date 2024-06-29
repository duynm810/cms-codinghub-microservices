using EventBus.IntegrationEvents;

namespace Post.Domain.Services;

public interface IPostEventService
{
    Task HandlePostCreatedEvent(PostCreatedEvent postCreatedEvent);
}