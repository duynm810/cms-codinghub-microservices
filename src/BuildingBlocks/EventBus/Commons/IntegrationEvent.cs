using EventBus.Commons.Interfaces;

namespace EventBus.Commons;

public record IntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.UtcNow;

    public string EventType { get; set; }
    
    public Guid? CorrelationId { get; set; }
    
    public string SourceService { get; set; }
    
    public IntegrationEvent(string sourceService)
    {
        EventType = GetType().Name;
        SourceService = sourceService;
    }
}