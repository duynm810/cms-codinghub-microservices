using EventBus.IntegrationEvents.Interfaces;
using MassTransit;
using PostInTag.Api.Repositories.Interfaces;
using ILogger = Serilog.ILogger;

namespace PostInTag.Api.Consumers.Tags;

public class PostTagsDeletedEventConsumer(IPostInTagRepository postInTagRepository, ILogger logger)
    : IConsumer<IPostTagsDeletedEvent>
{
    public async Task Consume(ConsumeContext<IPostTagsDeletedEvent> context)
    {
        var message = context.Message;

        const string methodName = nameof(Consume);
        const string className = nameof(PostTagsDeletedEventConsumer);
        
        logger.Information("BEGIN processing {ClassName} - PostId: {PostId}", className, message.PostId);

        try
        {
            await postInTagRepository.DeleteTagsByPostId(message.PostId);

            logger.Information("END processing {ClassName} successfully - PostId: {PostId}", className, message.PostId);
        }
        catch (Exception e)
        {
            logger.Error(e, "ERROR while processing {MethodName} - PostId: {PostId}. Error: {ErrorMessage}", methodName, message.PostId, e.Message);
            throw;
        }
    }
}