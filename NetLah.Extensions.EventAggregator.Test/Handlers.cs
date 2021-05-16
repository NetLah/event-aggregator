using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetLah.Extensions.EventAggregator.Test
{
    internal delegate Task Handler1Async<in TEvent>(TEvent arg1, IServiceProvider arg2, CancellationToken arg3);

    internal delegate void Handler2<in TEvent>(TEvent arg1, IServiceProvider arg2);

    internal delegate Task Handler3Async<in TEvent>(TEvent arg1, IServiceProvider arg2);

    internal delegate Task Handler4Async<in TEvent>(TEvent arg1);

    internal delegate void Handler5Async<in TEvent>(TEvent arg1);
}
