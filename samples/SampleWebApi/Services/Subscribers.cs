using NetLah.Extensions.EventAggregator;
using SampleWebApi.Models;

namespace SampleWebApi.Services;

public class Event1Subscriber : IAsyncSubscriber<Event1>
{
    private readonly ILogger _logger;

    public Event1Subscriber(ILogger<Event1Subscriber> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(Event1? @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handled[Event1Subscriber] {event1}", @event?.Message);
        return Task.CompletedTask;
    }
}

public class RootEvent1Subscriber : IAsyncSubscriber<Event1>
{
    private readonly ILogger _logger;

    public RootEvent1Subscriber(ILogger<RootEvent1Subscriber> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(Event1? @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handled[RootEvent1Subscriber] {event1}", @event?.Message);
        return Task.CompletedTask;
    }
}

public class Event2Subscriber : ISubscriber<BaseEvent2>
{
    private readonly ILogger _logger;

    public Event2Subscriber(ILogger<Event2Subscriber> logger)
    {
        _logger = logger;
    }

    public void Handle(BaseEvent2? @event)
    {
        _logger.LogInformation("Handled[Event2Subscriber] {event2}", @event?.Message2);
    }
}

public class RootEvent2Subscriber : IAsyncSubscriber<Event2>
{
    private readonly ILogger _logger;

    public RootEvent2Subscriber(ILogger<RootEvent2Subscriber> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(Event2? @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handled[RootEvent2Subscriber] {event2}", @event?.Message2);
        return Task.CompletedTask;
    }
}

public class Event3Subscriber : ISubscriber<IEvent3>
{
    private readonly ILogger _logger;

    public Event3Subscriber(ILogger<Event3Subscriber> logger)
    {
        _logger = logger;
    }

    public void Handle(IEvent3? @event)
    {
        _logger.LogInformation("Handled[Event3Subscriber]/{type} {event3}", @event?.GetType().Name, @event?.Message3);
    }
}

public class RootEvent3Subscriber
{
    private readonly ILogger _logger;

    public RootEvent3Subscriber(ILogger<RootEvent3Subscriber> logger)
    {
        _logger = logger;
    }

    public void Handle3(IEvent3? @event)
    {
        _logger.LogInformation("Handled[RootEvent3Subscriber]/{type} {event3}", @event?.GetType().Name, @event?.Message3);
    }
}
