using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace NetLah.Extensions.EventAggregator.Internal
{
    internal class SubscriptionFrom1Async<TEvent> : SubscriptionFrom1Async where TEvent : IEvent
    {
        public SubscriptionFrom1Async(Func<TEvent, IServiceProvider, CancellationToken, Task> handler)
            : base(typeof(TEvent), (o, sp, c) => handler((TEvent)o, sp, c))
        { }
    }

    internal class SubscriptionFrom2<TEvent> : SubscriptionFrom2 where TEvent : IEvent
    {
        public SubscriptionFrom2(Action<TEvent, IServiceProvider> handler)
            : base(typeof(TEvent), (o, sp) => handler((TEvent)o, sp))
        { }
    }

    internal sealed class SubscriptionFrom3Async<TEvent> : SubscriptionFrom3Async where TEvent : IEvent
    {
        public SubscriptionFrom3Async(Func<TEvent, IServiceProvider, Task> handler)
            : base(typeof(TEvent), (o, sp) => handler((TEvent)o, sp))
        { }
    }

    internal sealed class SubscriptionFrom4Async<TEvent> : SubscriptionFrom4Async where TEvent : IEvent
    {
        public SubscriptionFrom4Async(Func<TEvent, Task> handler)
            : base(typeof(TEvent), (o) => handler((TEvent)o))
        { }
    }

    internal sealed class SubscriptionFrom5<TEvent> : SubscriptionFrom5 where TEvent : IEvent
    {
        public SubscriptionFrom5(Action<TEvent> handler)
            : base(typeof(TEvent), (o) => handler((TEvent)o))
        { }
    }

    internal sealed class SubscriptionAsyncSubscriber<TEvent, TAsyncSubscriber> : SubscriptionFrom1Async<TEvent> where TEvent : IEvent where TAsyncSubscriber : IAsyncSubscriber<TEvent>
    {
        public SubscriptionAsyncSubscriber()
            : base((ev, sp, cancellationToken) => sp.GetRequiredService<TAsyncSubscriber>().HandleAsync(ev, cancellationToken))
        { }
    }

    internal sealed class SubscriptionSubscriber<TEvent, TSubscriber> : SubscriptionFrom2<TEvent> where TEvent : IEvent where TSubscriber : ISubscriber<TEvent>
    {
        public SubscriptionSubscriber()
            : base((ev, sp) => sp.GetRequiredService<TSubscriber>().Handle(ev))
        { }
    }
}
