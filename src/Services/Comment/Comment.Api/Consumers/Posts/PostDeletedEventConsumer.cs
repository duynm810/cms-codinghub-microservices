using Comment.Api.Repositories.Interfaces;
using EventBus.IntegrationEvents.Interfaces;
using MassTransit;
using ILogger = Serilog.ILogger;

namespace Comment.Api.Consumers.Posts;

public class PostDeletedEventConsumer(
    ICommentRepository commentRepository,
    ILogger logger) : IConsumer<IPostDeletedEvent>
{
    public async Task Consume(ConsumeContext<IPostDeletedEvent> context)
    {
        var message = context.Message;

        const string methodName = nameof(Consume);
        const string className = nameof(PostDeletedEventConsumer);

        logger.Information("BEGIN processing {ClassName} - PostId: {PostId}", className, message.PostId);

        try
        {
            await commentRepository.DeleteCommentsByPostId(message.PostId);

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