using System;
using NetLah.Extensions.EventAggregator.Internal;

namespace NetLah.Extensions.EventAggregator.Test
{
    internal class EACore : EventAggregatorCore
    {
        public EACore(IServiceProvider serviceProvider, EventAggregatorOptions options)
            : base(serviceProvider, () => options)
        {
        }
    }
}
