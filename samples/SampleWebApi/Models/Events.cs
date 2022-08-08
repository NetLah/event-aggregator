using NetLah.Extensions.EventAggregator;

namespace SampleWebApi.Models;

public class Event1 : IEvent
{
    public string? Message { get; set; }
}

public abstract class BaseEvent2 : IEvent
{
    public string? Message2 { get; set; }
}

public class Event2 : BaseEvent2
{
}

public interface IEvent3 : IEvent
{
    string? Message3 { get; }
}

public abstract class Event3 : IEvent3
{
    public string? Message3 { get; set; }
}

public sealed class Event3A : Event3
{
}

public sealed class Event3B : Event3
{
}
