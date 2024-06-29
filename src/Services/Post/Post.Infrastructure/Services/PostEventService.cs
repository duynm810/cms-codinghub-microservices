using EventBus.IntegrationEvents;
using EventBus.IntegrationEvents.Interfaces;
using MassTransit;
using Microsoft.Extensions.Options;
using Post.Domain.Services;
using Serilog;
using Shared.Dtos.Tag;
using Shared.Settings;

namespace Post.Infrastructure.Services;

public class PostEventService(IPublishEndpoint publishEndpoint, IOptions<EventBusSettings> eventBusSettings, ILogger logger) : IPostEventService
{
    private readonly EventBusSettings _eventBusSettings = eventBusSettings.Value;
    
    public async Task HandlePostCreatedEvent(Guid postId, List<RawTagDto> rawTags)
    {
        logger.Information("BEGIN Publish PostCreatedEvent - PostId: {PostId}", postId);

        try
        {
            var postCreatedEvent = new PostCreatedEvent(_eventBusSettings.ServiceName)
            {
                PostId = postId,
                RawTags = rawTags
            };
            
            await publishEndpoint.Publish<IPostCreatedEvent>(postCreatedEvent);
            logger.Information("END Publish PostCreatedEvent successfully - PostId: {PostId}", postId);
        }
        catch (Exception e)
        {
            logger.Error(e, "ERROR while publishing PostCreatedEvent - PostId: {PostId}", postId);
            throw;
        }
    }
}