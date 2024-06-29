using EventBus.Commons.Interfaces;

namespace EventBus.IntegrationEvents.Interfaces;

public interface ITagProcessedEvent : IIntegrationEvent
{
    public Guid PostId { get; set; }

    public List<Guid> TagIds { get; set; }
}