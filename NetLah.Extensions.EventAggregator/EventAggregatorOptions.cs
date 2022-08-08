using NetLah.Extensions.EventAggregator.Internal;
using System.Collections.Concurrent;

namespace NetLah.Extensions.EventAggregator;

public class EventAggregatorOptions
{
    internal List<Subscription> Subscriptions { get; } = new();

    internal ConcurrentDictionary<Type, Lazy<Subscription[]>> Handlers { get; } = new();
}
