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
        var emailEvent = context.Message;

        var jobId = backgroundJobService.SendEmail(emailEvent.To, emailEvent.Subject, emailEvent.EmailContent,
            emailEvent.EnqueueAt);

        if (jobId != null)
        {
            logger.Information("Processed PostRejectedWithReasonEvent - Event Id: {EventId}, Job Id: {JobId}",
                emailEvent.Id, jobId);
        }
        else
        {
            logger.Warning("Failed to process PostRejectedWithReasonEvent - Event Id: {EventId}", emailEvent.Id);
        }

        await Task.CompletedTask;
    }
}