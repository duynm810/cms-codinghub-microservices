using EventBus.IntegrationEvents.Posts;
using MassTransit;
using Post.Domain.Services;
using Serilog;

namespace Post.Infrastructure.Services;

public class PostEventService(IPublishEndpoint publishEndpoint, ILogger logger) : IPostEventService
{
    public async Task HandlePostCreatedEvent(PostCreatedEvent postCreatedEvent)
    {
        logger.Information("Handling PostCreatedEvent - PostId: {PostId}", postCreatedEvent.PostId);

        try
        {
            await publishEndpoint.Publish(postCreatedEvent);
            logger.Information("Handled PostCreatedEvent successfully - PostId: {PostId}", postCreatedEvent.PostId);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred while handling PostCreatedEvent - PostId: {PostId}", postCreatedEvent.PostId);
            throw;
        }
    }
}