namespace NetLah.Extensions.EventAggregator.Test;

internal static class Helper
{
    public static IServiceProvider NullServiceProvider() => Null<IServiceProvider>();

#pragma warning disable CS8603 // Possible null reference return.
    public static TService Null<TService>() where TService : class => null;
#pragma warning restore CS8603 // Possible null reference return.
}
