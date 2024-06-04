namespace EventBus.Commons.Interfaces;

public interface ISendEmailEvent : IIntegrationEvent
{
    string To { get; set; }

    string Subject { get; set; }

    string EmailContent { get; set; }

    DateTimeOffset EnqueueAt { get; set; }
}