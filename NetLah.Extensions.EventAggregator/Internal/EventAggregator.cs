using System;
using Microsoft.Extensions.Options;

namespace NetLah.Extensions.EventAggregator.Internal
{
    internal class EventAggregator : EventAggregatorCore
    {
        public EventAggregator(IServiceProvider serviceProvider, IOptionsMonitor<EventAggregatorOptions> optionsAccessor) :
            base(serviceProvider, () => optionsAccessor.Get(Options.DefaultName))
        {
        }
    }
}
