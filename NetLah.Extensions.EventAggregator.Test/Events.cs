namespace NetLah.Extensions.EventAggregator.Test;

internal class Event0 : IEvent
{
}

internal class Event1 : IEvent
{
}

internal class Event1Child : Event1
{
}

internal class Event2 : IEvent
{
}

internal class Event3 : IEvent
{
}

internal class Event4 : IEvent
{
}

internal class Event5 : IEvent
{
}

internal abstract class BaseAsyncSubscriber : IAsyncSubscriber<Event1>
{
    public abstract Task HandleAsync(Event1? @event, CancellationToken cancellationToken = default);
}

internal abstract class BaseSubscriber : ISubscriber<Event1>
{
    public abstract void Handle(Event1? @event);
}
