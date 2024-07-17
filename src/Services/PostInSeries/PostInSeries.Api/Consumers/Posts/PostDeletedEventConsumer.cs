using EventBus.IntegrationEvents.Interfaces;
using MassTransit;
using PostInSeries.Api.Repositories.Interfaces;
using ILogger = Serilog.ILogger;

namespace PostInSeries.Api.Consumers.Posts;

public class PostDeletedEventConsumer(
    IPostInSeriesRepository postInSeriesRepository,
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
            await postInSeriesRepository.DeletePostToSeries(message.PostId);
            
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