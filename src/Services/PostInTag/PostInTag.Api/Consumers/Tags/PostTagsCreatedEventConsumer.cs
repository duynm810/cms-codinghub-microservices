using AutoMapper;
using EventBus.IntegrationEvents.Interfaces;
using MassTransit;
using PostInTag.Api.Entities;
using PostInTag.Api.Repositories.Interfaces;
using Shared.Dtos.PostInTag;
using Shared.Requests.PostInTag;
using ILogger = Serilog.ILogger;

namespace PostInTag.Api.Consumers.Tags;

public class PostTagsCreatedEventConsumer(IPostInTagRepository postInTagRepository, IMapper mapper, ILogger logger)
    : IConsumer<IPostTagsCreatedEvent>
{
    public async Task Consume(ConsumeContext<IPostTagsCreatedEvent> context)
    {
        var message = context.Message;

        const string className = nameof(PostTagsCreatedEventConsumer);
        const string methodName = nameof(Consume);

        logger.Information("BEGIN processing {ClassName} - PostId: {PostId}", className, message.PostId);

        try
        {
            var sortOrder = 1;

            foreach (var tagId in message.TagIds)
            {
                var postInTagRequest = new CreatePostInTagRequest
                {
                    TagId = tagId,
                    PostId = message.PostId,
                    SortOrder = sortOrder++
                };

                var postInTag = mapper.Map<PostInTagBase>(postInTagRequest);
                await postInTagRepository.CreatePostToTag(postInTag);
            }

            logger.Information("END processing {ClassName} successfully - PostId: {PostId}", className,
                message.PostId);
        }
        catch (Exception e)
        {
            logger.Error(e, "ERROR while processing {MethodName} - PostId: {PostId}. Error: {ErrorMessage}", methodName,
                message.PostId, e.Message);
            throw;
        }
    }
}