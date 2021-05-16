# NetLah.Extensions.EventAggregator - .NET Library

[NetLah.Extensions.EventAggregator](https://www.nuget.org/packages/NetLah.Extensions.EventAggregator/) is a library which contains a set of reusable classes implement Event Aggregator pattern integrated with dependency injection `Microsoft.Extensions.DependencyInjection`. These classes/interface are `IEventAggregator`, `IRootEventAggregator`, `IAsyncSubscriber`, `ISubscriber`.

## Getting started

### Scoped and singleton

```
services.AddEventAggregator();

services.AddSingleton<RootEvent1Subscriber>();
services.SubscribeAsync<BaseEvent1, RootEvent1Subscriber>(lifetime: SubscriberLifetime.Singleton);

services.AddScoped<Event1Subscriber>();             // IAsyncSubscriber<TEvent>
services.AddScoped<Event2Subscriber>();             // ISubscriber<TEvent>
services.AddScoped<Event3Subscriber>();             // IAsyncSubscriber<TEvent>
services.AddScoped<Event4Subscriber>();             // ISubscriber<TEvent>

services.SubscribeAsync<Event1, Event1Subscriber>();
services.Subscribe<Event2, Event2Subscriber>();
services.Subscribe<IEvent3>((ev, sp) => sp.GetRequiredService<Event3Subscriber>().Handle(ev));
services.Subscribe<Event4>((ev, sp, ct) => sp.GetRequiredService<Event4Subscriber>().HandleAsync(ev, ct));

// IEventAggregator eventAggregator
// both RootEvent1Subscriber and Event1Subscriber subscribe on Event1
await eventAggregator.PublishAsync(new Event1 { Message = message1 });
await eventAggregator.PublishAsync(new Event2 { Payload = payload2 });
await eventAggregator.PublishAsync(new Event3());
await eventAggregator.PublishAsync(new Event4());
```

### Singleton only

```
services.AddEventAggregator();

services.AddSingleton<RootEvent1Subscriber>();      // IAsyncSubscriber<TEvent>
services.AddSingleton<RootEvent2Subscriber>();
services.SubscribeAsync<Event1, RootEvent1Subscriber>(lifetime: SubscriberLifetime.Singleton);
services.Subscribe<Event2>((ev, sp) => sp.GetRequiredService<RootEvent2Subscriber>().Handle(ev), lifetime: SubscriberLifetime.Singleton);

// IRootEventAggregator rootEventAggregator
await rootEventAggregator.PublishAsync(new Event1 { Message = message1 });
await rootEventAggregator.PublishAsync(new Event2 { Payload = payload2 });
```

## Interface subscriber

```
public interface IAsyncSubscriber<in TEvent> where TEvent : IEvent
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}

public interface ISubscriber<in TEvent> where TEvent : IEvent
{
    void Handle(TEvent @event);
}
```

## Delegate subscriber

```
public static IServiceCollection SubscribeAsync<TEvent>(this IServiceCollection services, Func<TEvent, IServiceProvider, CancellationToken, Task> handler, SubscriberLifetime lifetime = SubscriberLifetime.Scoped);

public static IServiceCollection SubscribeAsync<TEvent>(this IServiceCollection services, Func<TEvent, IServiceProvider, Task> handler, SubscriberLifetime lifetime = SubscriberLifetime.Scoped)

public static IServiceCollection Subscribe<TEvent>(this IServiceCollection services, Action<TEvent, IServiceProvider> handler, SubscriberLifetime lifetime = SubscriberLifetime.Scoped)
```
