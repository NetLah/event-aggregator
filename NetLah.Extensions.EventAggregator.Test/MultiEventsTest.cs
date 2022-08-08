using Moq;
using Xunit;

namespace NetLah.Extensions.EventAggregator.Test;

public class MultiEventsTest
{
    [Fact]
    public async Task Event1ThenEvent2_Success()
    {
        var (handler1Mock, handler2Mock, service) = Setup1();

        var e1 = new Event1();
        await service.PublishAsync(e1);
        handler1Mock.Verify(d => d(e1, Helper.NullServiceProvider(), It.IsAny<CancellationToken>()), Times.Once);
        handler2Mock.Verify(d => d(It.IsAny<Event2>(), Helper.NullServiceProvider(), It.IsAny<CancellationToken>()), Times.Never);

        var e2 = new Event2();
        await service.PublishAsync(e2);
        handler1Mock.Verify(d => d(e1, Helper.NullServiceProvider(), It.IsAny<CancellationToken>()), Times.Once);
        handler2Mock.Verify(d => d(e2, Helper.NullServiceProvider(), It.IsAny<CancellationToken>()), Times.Once);

        // reset
        (handler1Mock, handler2Mock, service) = Setup1();

        var e2b = new Event2();
        await service.PublishAsync(e2b);
        handler1Mock.Verify(d => d(It.IsAny<Event1>(), Helper.NullServiceProvider(), It.IsAny<CancellationToken>()), Times.Never);
        handler2Mock.Verify(d => d(e2, Helper.NullServiceProvider(), It.IsAny<CancellationToken>()), Times.Never);
        handler2Mock.Verify(d => d(e2b, Helper.NullServiceProvider(), It.IsAny<CancellationToken>()), Times.Once);

        var e1b = new Event1();
        await service.PublishAsync(e1b);
        handler1Mock.Verify(d => d(e1, Helper.NullServiceProvider(), It.IsAny<CancellationToken>()), Times.Never);
        handler2Mock.Verify(d => d(e2, Helper.NullServiceProvider(), It.IsAny<CancellationToken>()), Times.Never);
        handler1Mock.Verify(d => d(e1b, Helper.NullServiceProvider(), It.IsAny<CancellationToken>()), Times.Once);
        handler2Mock.Verify(d => d(e2b, Helper.NullServiceProvider(), It.IsAny<CancellationToken>()), Times.Once);

        static (Mock<Handler1Async<Event1?>>, Mock<Handler1Async<Event2?>>, EACore) Setup1()
        {
            var handler1Mock = new Mock<Handler1Async<Event1?>>();
            var handler2Mock = new Mock<Handler1Async<Event2?>>();
            var options = new EventAggregatorOptions();
            options.AddHandler1<Event1>(handler1Mock.Object.Invoke);
            options.AddHandler1<Event2>(handler2Mock.Object.Invoke);
            var service = new EACore(Helper.NullServiceProvider(), options);
            return (handler1Mock, handler2Mock, service);
        }
    }
}
