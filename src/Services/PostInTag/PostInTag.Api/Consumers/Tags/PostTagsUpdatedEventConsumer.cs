using AutoMapper;
using EventBus.IntegrationEvents.Interfaces;
using MassTransit;
using PostInTag.Api.Entities;
using PostInTag.Api.Repositories.Interfaces;
using Shared.Dtos.PostInTag;
using ILogger = Serilog.ILogger;

namespace PostInTag.Api.Consumers.Tags;

public class PostTagsUpdatedEventConsumer(IPostInTagRepository postInTagRepository, IMapper mapper, ILogger logger)
    : IConsumer<IPostTagsUpdatedEvent>
{
    public async Task Consume(ConsumeContext<IPostTagsUpdatedEvent> context)
    {
        var message = context.Message;

        const string className = nameof(PostTagsUpdatedEventConsumer);
        const string methodName = nameof(Consume);

        logger.Information("BEGIN processing {ClassName} - PostId: {PostId}", className, message.PostId);

        try
        {
            // Remove tags
            foreach (var tagId in message.TagsToRemove)
            {
                var postInTag = await postInTagRepository.GetPostInTag(message.PostId, tagId);
                if (postInTag != null)
                {
                    await postInTagRepository.DeletePostToTag(postInTag);
                }
            }

            // Insert new tags
            var sortOrder = 1;
            foreach (var tagId in message.NewTagIds)
            {
                var postInTagDto = new CreatePostInTagDto
                {
                    TagId = tagId,
                    PostId = message.PostId,
                    SortOrder = sortOrder++
                };

                var postInTag = mapper.Map<PostInTagBase>(postInTagDto);
                await postInTagRepository.CreatePostToTag(postInTag);
            }

            logger.Information("END processing {ClassName} successfully - PostId: {PostId}", className, message.PostId);
        }
        catch (Exception e)
        {
            logger.Error(e, "ERROR while processing {MethodName} - PostId: {PostId}. Error: {ErrorMessage}", methodName,
                message.PostId, e.Message);
            throw;
        }
    }
}