using EventBus.IntegrationEvents;
using EventBus.IntegrationEvents.Interfaces;
using MassTransit;
using Post.Domain.Services;
using Serilog;

namespace Post.Infrastructure.Services;

public class PostEventService(IPublishEndpoint publishEndpoint, ILogger logger) : IPostEventService
{
    public async Task HandlePostCreatedEvent(PostCreatedEvent postCreatedEvent)
    {
        logger.Information("BEGIN Publish PostCreatedEvent - PostId: {PostId}", postCreatedEvent.PostId);

        try
        {
            await publishEndpoint.Publish<IPostCreatedEvent>(postCreatedEvent);
            logger.Information("END Publish PostCreatedEvent successfully - PostId: {PostId}", postCreatedEvent.PostId);
        }
        catch (Exception e)
        {
            logger.Error(e, "ERROR while publishing PostCreatedEvent - PostId: {PostId}", postCreatedEvent.PostId);
            throw;
        }
    }
}