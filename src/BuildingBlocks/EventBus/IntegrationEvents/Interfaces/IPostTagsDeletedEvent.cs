using EventBus.Commons.Interfaces;

namespace EventBus.IntegrationEvents.Interfaces;

public interface IPostTagsDeletedEvent : IIntegrationEvent
{
    public Guid PostId { get; set; }
}