using Moq;
using Xunit;

namespace NetLah.Extensions.EventAggregator.Test;

public class EventTypeHandlerTest
{
    [Fact]
    public async Task EventAbstraction_NullEvent()
    {
        var service = new EACore(null, new EventAggregatorOptions());

#pragma warning disable CS8631 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match constraint type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        _ = await Assert.ThrowsAnyAsync<Exception>(() => service.PublishAsync((IEvent)null));
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8631 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match constraint type.
    }

    [Fact]
    public async Task EventAbstraction_ActualType_Success()
    {
        var handlerMock = new Mock<Action<Event1>>();
        var options = new EventAggregatorOptions();
        options.AddHandler5(handlerMock.Object);
        var service = new EACore(null, options);

        IEvent ev = new Event1();
        await service.PublishAsync(ev);

        handlerMock.Verify(d => d((Event1)ev), Times.Once);
    }

    [Fact]
    public async Task NullEvent_Success()
    {
        var handlerMock = new Mock<Action<Event1>>();
        var options = new EventAggregatorOptions();
        options.AddHandler5(handlerMock.Object);
        var service = new EACore(null, options);

        IEvent ev = null;
        await service.PublishAsync(ev);

        handlerMock.Verify(d => d(It.IsAny<Event1>()), Times.Never);
    }

    [Fact]
    public async Task EventChild_Success()
    {
        var handlerMock = new Mock<Action<Event1>>();
        var options = new EventAggregatorOptions();
        options.AddHandler5(handlerMock.Object);
        var service = new EACore(null, options);

        var ev = new Event1Child();
        await service.PublishAsync(ev);

        handlerMock.Verify(d => d(ev), Times.Once);
    }

}
