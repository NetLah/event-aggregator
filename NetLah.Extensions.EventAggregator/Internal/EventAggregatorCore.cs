namespace NetLah.Extensions.EventAggregator.Internal;

internal abstract class EventAggregatorCore : IEventAggregator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Func<EventAggregatorOptions> _optionsFactory;
    private EventAggregatorOptions? _options;

    protected EventAggregatorCore(IServiceProvider serviceProvider, Func<EventAggregatorOptions> optionsFactory)
    {
        _serviceProvider = serviceProvider;
        _optionsFactory = optionsFactory;
    }

    private EventAggregatorOptions OptionsValue => _options ??= _optionsFactory();

    private Subscription[] SubscriptionsFactory(Type type)
        => OptionsValue
            .Subscriptions
            .Where(s => s.Type.IsAssignableFrom(type))
            .ToArray();

#pragma warning disable S4457 // Parameter validation in "async"/"await" methods should be wrapped
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
#pragma warning restore S4457 // Parameter validation in "async"/"await" methods should be wrapped
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event));

        var type = @event.GetType();

#if !NETSTANDARD2_0
        var subscriptions = OptionsValue.Handlers.GetOrAdd<Func<Subscription[]>>(
            type,
            (type, factory) => new Lazy<Subscription[]>(factory),
            () => SubscriptionsFactory(type));
#else
        var subscriptions = OptionsValue.Handlers.GetOrAdd(
            type,
            type => new Lazy<Subscription[]>(
                () => SubscriptionsFactory(type)));
#endif

        foreach (var subscription in subscriptions.Value)
        {
            switch (subscription)
            {
                case SubscriptionFrom1Async handler:
                    await handler.HandleAsync(@event, _serviceProvider, cancellationToken).ConfigureAwait(false);
                    break;

                case SubscriptionFrom2 handler:
                    handler.Handle(@event, _serviceProvider);
                    break;

                case SubscriptionFrom3Async handler:
                    await handler.HandleAsync(@event, _serviceProvider).ConfigureAwait(false);
                    break;

                case SubscriptionFrom4Async handler:
                    await handler.HandleAsync(@event).ConfigureAwait(false);
                    break;

                case SubscriptionFrom5 handler:
                    handler.Handle(@event);
                    break;

                default:
                    throw new NotSupportedException($"EA not support type:{type.FullName}, expected type:{subscription.Type.FullName}");
            }
        }
    }
}
