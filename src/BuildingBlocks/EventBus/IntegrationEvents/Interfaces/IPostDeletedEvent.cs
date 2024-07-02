using EventBus.Commons.Interfaces;

namespace EventBus.IntegrationEvents.Interfaces;

public interface IPostDeletedEvent : IIntegrationEvent
{
    public Guid PostId { get; set; }
}