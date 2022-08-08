namespace NetLah.Extensions.EventAggregator;

public interface IAsyncSubscriber<in TEvent> where TEvent : IEvent
{
    Task HandleAsync(TEvent? @event, CancellationToken cancellationToken = default);
}
