using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetLah.Extensions.EventAggregator;
using NetLah.Extensions.EventAggregator.Internal;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventAggregatorExtensions
    {
        public static IServiceCollection AddEventAggregator(this IServiceCollection services)
        {
            services.TryAddSingleton<IRootEventAggregator, RootEventAggregator>();
            services.TryAddScoped<IEventAggregator, EventAggregator>();
            return services;
        }

        public static IServiceCollection SubscribeAsync<TEvent>(this IServiceCollection services, Func<TEvent, IServiceProvider, CancellationToken, Task> handler, SubscriberLifetime lifetime = SubscriberLifetime.Scoped)
            where TEvent : IEvent
            => services.Subscribe(new SubscriptionFrom1Async<TEvent>(handler), lifetime);

        public static IServiceCollection SubscribeAsync<TEvent>(this IServiceCollection services, Func<TEvent, IServiceProvider, Task> handler, SubscriberLifetime lifetime = SubscriberLifetime.Scoped)
            where TEvent : IEvent
            => services.Subscribe(new SubscriptionFrom3Async<TEvent>(handler), lifetime);

        public static IServiceCollection SubscribeAsync<TEvent, TAsyncSubscriber>(this IServiceCollection services, SubscriberLifetime lifetime = SubscriberLifetime.Scoped)
            where TEvent : IEvent
            where TAsyncSubscriber : IAsyncSubscriber<TEvent>
            => services.Subscribe(new SubscriptionAsyncSubscriber<TEvent, TAsyncSubscriber>(), lifetime);

        public static IServiceCollection Subscribe<TEvent>(this IServiceCollection services, Action<TEvent, IServiceProvider> handler, SubscriberLifetime lifetime = SubscriberLifetime.Scoped)
            where TEvent : IEvent
            => services.Subscribe(new SubscriptionFrom2<TEvent>(handler), lifetime);

        public static IServiceCollection Subscribe<TEvent, TSubscriber>(this IServiceCollection services, SubscriberLifetime lifetime = SubscriberLifetime.Scoped)
            where TEvent : IEvent
            where TSubscriber : ISubscriber<TEvent>
            => services.Subscribe(new SubscriptionSubscriber<TEvent, TSubscriber>(), lifetime);

        private static IServiceCollection Subscribe(this IServiceCollection services, Subscription subscription, SubscriberLifetime lifetime)
            => services.Configure<EventAggregatorOptions>(NamedScopedOrSingleton(lifetime), options => options.Subscriptions.Add(subscription));

        private static string NamedScopedOrSingleton(SubscriberLifetime lifetime) => lifetime == SubscriberLifetime.Scoped ? Options.Options.DefaultName : null;
    }
}
