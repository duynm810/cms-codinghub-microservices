using EventBus.Events;
using EventBus.IntergrationEvents.Interfaces;

namespace EventBus.IntergrationEvents;

public record PostRejectedWithReasonEvent : SendEmailEvent , IPostRejectedWithReasonEvent;