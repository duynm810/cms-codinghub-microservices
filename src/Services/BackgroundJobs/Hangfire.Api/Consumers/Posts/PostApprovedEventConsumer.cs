using EventBus.IntergrationEvents;
using Hangfire.Api.Services.Interfaces;
using MassTransit;
using ILogger = Serilog.ILogger;

namespace Hangfire.Api.Consumers.Posts;

public class PostApprovedEventConsumer(IBackgroundJobService backgroundJobService, ILogger logger)
    : IConsumer<PostApprovedEvent>
{
    public async Task Consume(ConsumeContext<PostApprovedEvent> context)
    {
        var emailEvent = context.Message;

        var jobId = backgroundJobService.SendEmail(emailEvent.To, emailEvent.Subject, emailEvent.EmailContent,
            emailEvent.EnqueueAt);

        if (jobId != null)
        {
            logger.Information("Processed PostApprovedEvent - Event Id: {EventId}, Job Id: {JobId}", emailEvent.Id,
                jobId);
        }
        else
        {
            logger.Warning("Failed to process PostApprovedEvent - Event Id: {EventId}", emailEvent.Id);
        }

        await Task.CompletedTask;
    }
}