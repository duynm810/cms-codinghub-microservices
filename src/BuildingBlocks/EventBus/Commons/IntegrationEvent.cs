using EventBus.Commons.Interfaces;

namespace EventBus.Commons;

public record IntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public DateTimeOffset CreationDate { get; set; }  = DateTimeOffset.UtcNow;
}