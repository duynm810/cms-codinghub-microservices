using EventBus.IntegrationEvents;
using EventBus.IntegrationEvents.Interfaces;
using MassTransit;
using Microsoft.Extensions.Options;
using Shared.Settings;
using Tag.Api.GrpcClients.Interfaces;
using Tag.Api.Repositories.Interfaces;
using ILogger = Serilog.ILogger;

namespace Tag.Api.Consumers.Posts;

public class PostDeletedEventConsumer(
    IPublishEndpoint publishEndpoint,
    ITagRepository tagRepository,
    IPostInTagGrpcClient postInTagGrpcClient,
    IOptions<EventBusSettings> eventBusSettings,
    ILogger logger) : IConsumer<IPostDeletedEvent>
{
    private readonly EventBusSettings _eventBusSettings = eventBusSettings.Value;

    public async Task Consume(ConsumeContext<IPostDeletedEvent> context)
    {
        var message = context.Message;

        const string methodName = nameof(Consume);
        const string className = nameof(PostDeletedEventConsumer);

        logger.Information("BEGIN processing {ClassName} - PostId: {PostId}", className, message.PostId);

        await using var transaction = await tagRepository.BeginTransactionAsync();

        try
        {
            var tagIds = await postInTagGrpcClient.GetTagIdsByPostIdAsync(message.PostId);
            var tagIdList = tagIds.ToList();
            if (tagIdList.Count != 0)
            {
                var tags = await tagRepository.GetTagsByIds(tagIdList);

                foreach (var tag in tags)
                {
                    tag.UsageCount--;
                    await tagRepository.UpdateTag(tag);
                }
            }

            await transaction.CommitAsync();

            // Publish event for processed tags (Phát hành sự kiện cho các tags đã được xử lý)
            await PublishPostTagsDeletedEvent(message.PostId);

            logger.Information("END processing {ClassName} successfully - PostId: {PostId}", className, message.PostId);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.Error(e,
                "ERROR while processing {MethodName} - PostId: {PostId}. Error: {ErrorMessage}", methodName,
                message.PostId, e.Message);
            throw;
        }
    }

    #region HELPERS

    private async Task PublishPostTagsDeletedEvent(Guid postId)
    {
        const string methodName = nameof(PublishPostTagsDeletedEvent);

        var serviceName = _eventBusSettings.ServiceName;

        logger.Information("BEGIN Publish {MethodName} - PostId: {PostId}, SourceService: {SourceService}", methodName,
            postId, serviceName);

        try
        {
            var postTagsDeletedEvent = new PostTagsDeletedEvent(_eventBusSettings.ServiceName)
            {
                PostId = postId
            };

            await publishEndpoint.Publish<IPostTagsDeletedEvent>(postTagsDeletedEvent);

            logger.Information(
                "END Publish {MethodName} successfully - PostId: {PostId}, SourceService: {SourceService}", methodName,
                postId, serviceName);
        }
        catch (Exception e)
        {
            logger.Error(e,
                "ERROR while publishing {MethodName} - PostId: {PostId}. Error: {ErrorMessage}",
                methodName, postId, e.Message);
            throw;
        }
    }

    #endregion
}