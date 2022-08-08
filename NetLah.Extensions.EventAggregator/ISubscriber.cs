namespace NetLah.Extensions.EventAggregator;

public interface ISubscriber<in TEvent> where TEvent : IEvent
{
    void Handle(TEvent @event);
}
