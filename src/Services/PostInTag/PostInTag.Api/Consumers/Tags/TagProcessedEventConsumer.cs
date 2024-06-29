using AutoMapper;
using EventBus.IntegrationEvents.Interfaces;
using MassTransit;
using PostInTag.Api.Entities;
using PostInTag.Api.Repositories.Interfaces;
using Shared.Dtos.PostInTag;
using ILogger = Serilog.ILogger;

namespace PostInTag.Api.Consumers.Tags;

public class TagProcessedEventConsumer(IPostInTagRepository postInTagRepository, IMapper mapper, ILogger logger) : IConsumer<ITagProcessedEvent>
{
    public async Task Consume(ConsumeContext<ITagProcessedEvent> context)
    {
        var tagProcessedEvent = context.Message;

        const string methodName = nameof(Consume);

        logger.Information("Handling TagProcessedEventConsumer - PostId: {PostId}", tagProcessedEvent.PostId);

        try
        {
            var sortOrder = 1;

            foreach (var tagId in tagProcessedEvent.TagIds)
            {
                var postInTagDto = new CreatePostInTagDto
                {
                    TagId = tagId,
                    PostId = tagProcessedEvent.PostId,
                    SortOrder = sortOrder++
                };

                var postInTag = mapper.Map<PostInTagBase>(postInTagDto);
                await postInTagRepository.CreatePostToTag(postInTag);
            }
        }
        catch (Exception e)
        {
            logger.Error(e, "{MethodName} - An error occurred while handling TagProcessedEvent - PostId: {PostId}. Error: {ErrorMessage}", methodName, tagProcessedEvent.PostId, e.Message);
            throw;
        }
    }
}