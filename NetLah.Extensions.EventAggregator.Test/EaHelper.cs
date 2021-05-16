using System;
using System.Threading;
using System.Threading.Tasks;
using NetLah.Extensions.EventAggregator.Internal;

namespace NetLah.Extensions.EventAggregator.Test
{
    internal static class EAHelper
    {
        public static EventAggregatorOptions AddHandler1<TEvent>(this EventAggregatorOptions options, Func<TEvent, IServiceProvider, CancellationToken, Task> handler)
            where TEvent : IEvent
            => options.AddSubscription(new SubscriptionFrom1Async<TEvent>(handler));

        public static EventAggregatorOptions AddHandler2<TEvent>(this EventAggregatorOptions options, Action<TEvent, IServiceProvider> handler)
            where TEvent : IEvent
             => options.AddSubscription(new SubscriptionFrom2<TEvent>(handler));

        public static EventAggregatorOptions AddHandler3<TEvent>(this EventAggregatorOptions options, Func<TEvent, IServiceProvider, Task> handler)
            where TEvent : IEvent
               => options.AddSubscription(new SubscriptionFrom3Async<TEvent>(handler));

        public static EventAggregatorOptions AddHandler4<TEvent>(this EventAggregatorOptions options, Func<TEvent, Task> handler)
            where TEvent : IEvent
               => options.AddSubscription(new SubscriptionFrom4Async<TEvent>(handler));

        public static EventAggregatorOptions AddHandler5<TEvent>(this EventAggregatorOptions options, Action<TEvent> handler)
            where TEvent : IEvent
              => options.AddSubscription(new SubscriptionFrom5<TEvent>(handler));

        public static EventAggregatorOptions AddHandler6<TEvent, TAsyncSubscriber>(this EventAggregatorOptions options)
            where TEvent : IEvent
            where TAsyncSubscriber : IAsyncSubscriber<TEvent>
             => options.AddSubscription(new SubscriptionAsyncSubscriber<TEvent, TAsyncSubscriber>());

        public static EventAggregatorOptions AddHandler7<TEvent, TSubscriber>(this EventAggregatorOptions options)
            where TEvent : IEvent
            where TSubscriber : ISubscriber<TEvent>
             => options.AddSubscription(new SubscriptionSubscriber<TEvent, TSubscriber>());

        private static EventAggregatorOptions AddSubscription(this EventAggregatorOptions options, Subscription subscription)
        {
            options.Subscriptions.Add(subscription);
            return options;
        }
    }
}
