using EventBus.Commons.Interfaces;

namespace EventBus.Commons;

public record IntergrationEvent : IIntergrationEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public DateTimeOffset CreationDate { get; set; }  = DateTimeOffset.UtcNow;
}