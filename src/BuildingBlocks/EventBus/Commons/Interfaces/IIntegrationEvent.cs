namespace EventBus.Commons.Interfaces;

public interface IIntegrationEvent
{
    Guid Id { get; set; }

    DateTimeOffset CreationDate { get; set; }
}