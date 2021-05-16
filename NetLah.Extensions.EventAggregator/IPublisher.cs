using System.Threading;
using System.Threading.Tasks;

namespace NetLah.Extensions.EventAggregator
{
    public interface IPublisher
    {
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent;
    }
}
