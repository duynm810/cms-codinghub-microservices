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
        const string methodName = nameof(HandlePostCreatedEvent);
        
        var serviceName = _eventBusSettings.ServiceName;

        logger.Information("BEGIN Publish {MethodName} - PostId: {PostId}, SourceService: {SourceService}", methodName,
            postId, serviceName);

        try
        {
            var postCreatedEvent = new PostCreatedEvent(serviceName)
            {
                PostId = postId,
                RawTags = rawTags
            };
            
            await publishEndpoint.Publish<IPostCreatedEvent>(postCreatedEvent);
            
            logger.Information(
                "END Publish {MethodName} successfully - PostId: {PostId}, SourceService: {SourceService}", methodName,
                postId, serviceName);
        }
        catch (Exception e)
        {
            logger.Error(e, "ERROR while publishing {MethodName} - PostId: {PostId}, SourceService: {SourceService}",
                methodName, postId, serviceName);
            throw;
        }
    }
}