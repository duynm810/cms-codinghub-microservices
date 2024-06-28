using EventBus.Commons.Interfaces;
using Shared.Dtos.Tag;

namespace EventBus.IntegrationEvents.Interfaces;

public interface IPostTagsProcessedEvent : IIntegrationEvent
{
    public Guid PostId { get; set; }

    public List<TagDto> Tags { get; set; }
}