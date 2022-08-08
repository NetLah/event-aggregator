namespace NetLah.Extensions.EventAggregator.Test;

internal class AsyncDisposableWrapper<TService> : IAsyncDisposable where TService : IDisposable
{
    public AsyncDisposableWrapper(TService service) => this.Service = service;

    public TService Service { get; }

    public ValueTask DisposeAsync()
    {
        if (Service is IAsyncDisposable asyncDisposable)
            return asyncDisposable.DisposeAsync();

        Service.Dispose();
#if NETCOREAPP3_1
        return new ValueTask(Task.CompletedTask);
#else
        return ValueTask.CompletedTask;
#endif
    }
}
