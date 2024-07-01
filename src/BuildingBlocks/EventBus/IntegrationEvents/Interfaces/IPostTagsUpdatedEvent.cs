using EventBus.Commons.Interfaces;

namespace EventBus.IntegrationEvents.Interfaces;

public interface IPostTagsUpdatedEvent : IIntegrationEvent
{
    public Guid PostId { get; set; }

    public List<Guid> NewTagIds { get; set; }

    public List<Guid> TagsToRemove { get; set; }
}