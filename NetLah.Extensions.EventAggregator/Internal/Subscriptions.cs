using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetLah.Extensions.EventAggregator.Internal
{
    internal abstract class Subscription
    {
        protected Subscription(Type type) => Type = type ?? throw new ArgumentNullException(nameof(type));
        public Type Type { get; }
    }

    internal class SubscriptionFrom1Async : Subscription
    {
        public SubscriptionFrom1Async(Type type, Func<object, IServiceProvider, CancellationToken, Task> handler) : base(type) => HandleAsync = handler;
        public Func<object, IServiceProvider, CancellationToken, Task> HandleAsync { get; }
    }

    internal class SubscriptionFrom2 : Subscription
    {
        public SubscriptionFrom2(Type type, Action<object, IServiceProvider> handler) : base(type) => Handle = handler;
        public Action<object, IServiceProvider> Handle { get; }
    }

    internal class SubscriptionFrom3Async : Subscription
    {
        public SubscriptionFrom3Async(Type type, Func<object, IServiceProvider, Task> handler) : base(type) => HandleAsync = handler;
        public Func<object, IServiceProvider, Task> HandleAsync { get; }
    }

    internal class SubscriptionFrom4Async : Subscription
    {
        public SubscriptionFrom4Async(Type type, Func<object, Task> handler) : base(type) => HandleAsync = handler;
        public Func<object, Task> HandleAsync { get; }
    }

    internal class SubscriptionFrom5 : Subscription
    {
        public SubscriptionFrom5(Type type, Action<object> handler) : base(type) => Handle = handler;
        public Action<object> Handle { get; }
    }
}
