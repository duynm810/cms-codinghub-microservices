using EventBus.Commons;
using EventBus.Commons.Interfaces;

namespace EventBus.Events.Interfaces;

public interface ISendEmailEvent : IIntergrationEvent
{
    string To { get; set; }
    
    string Subject { get; set; }
    
    string EmailContent { get; set; }
    
    DateTimeOffset EnqueueAt { get; set; }
}