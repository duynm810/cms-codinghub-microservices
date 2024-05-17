using EventBus.Events;
using Hangfire.Api.Services.Interfaces;
using MassTransit;

namespace Hangfire.Api.Consumers;

public class SendEmailEventConsumer(IBackgroundJobService backgroundJobService) : IConsumer<SendEmailEvent>
{
    public Task Consume(ConsumeContext<SendEmailEvent> context)
    {
        var emailEvent = context.Message;
        
        backgroundJobService.SendEmail(emailEvent.To, emailEvent.Subject, emailEvent.EmailContent, emailEvent.EnqueueAt);

        return Task.CompletedTask;
    }
}