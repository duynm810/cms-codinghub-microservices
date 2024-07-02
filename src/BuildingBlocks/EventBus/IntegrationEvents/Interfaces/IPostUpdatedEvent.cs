using EventBus.Commons.Interfaces;
using Shared.Dtos.Tag;

namespace EventBus.IntegrationEvents.Interfaces;

public interface IPostUpdatedEvent : IIntegrationEvent
{
    public Guid PostId { get; set; }

    public List<RawTagDto> RawTags { get; set; }
}