using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NetLah.Extensions.EventAggregator.Internal;

namespace NetLah.Extensions.EventAggregator
{
    public class EventAggregatorOptions
    {
        internal List<Subscription> Subscriptions { get; } = new();

        internal ConcurrentDictionary<Type, Lazy<Subscription[]>> Handlers { get; } = new();
    }
}
