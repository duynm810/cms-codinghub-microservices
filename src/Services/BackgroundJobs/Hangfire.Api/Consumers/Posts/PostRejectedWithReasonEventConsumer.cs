using EventBus.IntegrationEvents.Interfaces;
using Hangfire.Api.Services.Interfaces;
using MassTransit;
using ILogger = Serilog.ILogger;

namespace Hangfire.Api.Consumers.Posts;

public class PostRejectedWithReasonEventConsumer(IBackgroundJobService backgroundJobService, ILogger logger)
    : IConsumer<IPostRejectedWithReasonEvent>
{
    public async Task Consume(ConsumeContext<IPostRejectedWithReasonEvent> context)
    {
        var message = context.Message;

        var jobId = backgroundJobService.SendEmail(message.To, message.Subject, message.EmailContent,
            message.EnqueueAt);

        if (jobId != null)
        {
            logger.Information("Processed PostRejectedWithReasonEvent - Event Id: {EventId}, Job Id: {JobId}",
                message.Id, jobId);
        }
        else
        {
            logger.Warning("Failed to process PostRejectedWithReasonEvent - Event Id: {EventId}", message.Id);
        }

        await Task.CompletedTask;
    }
}