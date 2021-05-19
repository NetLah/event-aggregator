using System;
using System.Threading.Tasks;

namespace NetLah.Extensions.EventAggregator.Test
{
    internal class AsyncDisposableWrapper<TService> : IAsyncDisposable where TService : IDisposable
    {
        public AsyncDisposableWrapper(TService service) => this.Service = service;

        public TService Service { get; }

        public ValueTask DisposeAsync()
        {
            if (Service is IAsyncDisposable asyncDisposable)
                return asyncDisposable.DisposeAsync();

            Service.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
