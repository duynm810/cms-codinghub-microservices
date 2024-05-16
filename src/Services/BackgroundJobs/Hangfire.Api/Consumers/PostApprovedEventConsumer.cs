using EventBus.Events;
using MassTransit;

namespace Hangfire.Api.Consumers;

public class PostApprovedEventConsumer : IConsumer<PostApprovedEvent>
{
    public async Task Consume(ConsumeContext<PostApprovedEvent> context)
    {
        var postEvent = context.Message;
    }
}