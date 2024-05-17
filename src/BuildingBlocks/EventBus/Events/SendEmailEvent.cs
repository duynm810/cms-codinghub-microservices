using EventBus.Commons;
using EventBus.Events.Interfaces;

namespace EventBus.Events;

public record SendEmailEvent : IntergrationEvent, ISendEmailEvent
{
    public required string To { get; set; }
    
    public required string Subject { get; set; }
    
    public required string EmailContent { get; set; }
    
    public DateTimeOffset EnqueueAt { get; set; }
    
}