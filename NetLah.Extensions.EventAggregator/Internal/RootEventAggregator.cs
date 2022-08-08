using Microsoft.Extensions.Options;

namespace NetLah.Extensions.EventAggregator.Internal;

internal class RootEventAggregator : EventAggregatorCore, IRootEventAggregator
{
    public RootEventAggregator(IServiceProvider serviceProvider, IOptionsMonitor<EventAggregatorOptions> optionsAccessor) :
        base(serviceProvider, () => optionsAccessor.Get("Root"))
    {
    }
}
