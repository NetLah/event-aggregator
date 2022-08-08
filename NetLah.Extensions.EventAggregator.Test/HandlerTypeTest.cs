using Moq;
using Xunit;

namespace NetLah.Extensions.EventAggregator.Test;

public class HandlerTypeTest
{
    [Fact]
    public async Task DelegateForm1_Success()
    {
        var handlerMock = new Mock<Func<Event1, IServiceProvider, CancellationToken, Task>>();
        var options = new EventAggregatorOptions();
        options.AddHandler1(handlerMock.Object);
        var service = new EACore(null, options);

        await service.PublishAsync(new Event1());

        handlerMock.Verify(d => d(It.IsAny<Event1>(), null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DelegateForm2_Success()
    {
        var handlerMock = new Mock<Action<Event1, IServiceProvider>>();
        var options = new EventAggregatorOptions();
        options.AddHandler2(handlerMock.Object);
        var service = new EACore(null, options);

        await service.PublishAsync(new Event1());

        handlerMock.Verify(d => d(It.IsAny<Event1>(), null), Times.Once);
    }

    [Fact]
    public async Task DelegateForm3_Success()
    {
        var handlerMock = new Mock<Func<Event1, IServiceProvider, Task>>();
        var options = new EventAggregatorOptions();
        options.AddHandler3(handlerMock.Object);
        var service = new EACore(null, options);

        await service.PublishAsync(new Event1());

        handlerMock.Verify(d => d(It.IsAny<Event1>(), null), Times.Once);
    }

    [Fact]
    public async Task DelegateForm4_Success()
    {
        var handlerMock = new Mock<Func<Event1, Task>>();
        var options = new EventAggregatorOptions();
        options.AddHandler4(handlerMock.Object);
        var service = new EACore(null, options);

        await service.PublishAsync(new Event1());

        handlerMock.Verify(d => d(It.IsAny<Event1>()), Times.Once);
    }

    [Fact]
    public async Task DelegateForm5_Success()
    {
        var handlerMock = new Mock<Action<Event1>>();
        var options = new EventAggregatorOptions();
        options.AddHandler5(handlerMock.Object);
        var service = new EACore(null, options);

        await service.PublishAsync(new Event1());

        handlerMock.Verify(d => d(It.IsAny<Event1>()), Times.Once);
    }

    [Fact]
    public async Task AsyncSubscriberForm6_Success()
    {
        var handlerMock = new Mock<BaseAsyncSubscriber>();
        var spMock = new Mock<IServiceProvider>();
        spMock.Setup(sp => sp.GetService(typeof(BaseAsyncSubscriber))).Returns(handlerMock.Object).Verifiable();

        var options = new EventAggregatorOptions();
        options.AddHandler6<Event1, BaseAsyncSubscriber>();
        var service = new EACore(spMock.Object, options);

        await service.PublishAsync(new Event1());

        handlerMock.Verify(s => s.HandleAsync(It.IsAny<Event1>(), It.IsAny<CancellationToken>()), Times.Once);
        spMock.VerifyAll();
    }

    [Fact]
    public async Task SubscriberForm7_Success()
    {
        var handlerMock = new Mock<BaseSubscriber>();
        var spMock = new Mock<IServiceProvider>();
        spMock.Setup(sp => sp.GetService(typeof(BaseSubscriber))).Returns(handlerMock.Object).Verifiable();

        var options = new EventAggregatorOptions();
        options.AddHandler7<Event1, BaseSubscriber>();
        var service = new EACore(spMock.Object, options);

        await service.PublishAsync(new Event1());

        handlerMock.Verify(s => s.Handle(It.IsAny<Event1>()), Times.Once);
        spMock.VerifyAll();
    }

    [Fact]
    public async Task SubscriberForm7_NoService()
    {
        var spMock = new Mock<IServiceProvider>();

        var options = new EventAggregatorOptions();
        options.AddHandler7<Event1, BaseSubscriber>();
        var service = new EACore(spMock.Object, options);

        var ex = await Assert.ThrowsAnyAsync<InvalidOperationException>(() => service.PublishAsync(new Event1()));

        spMock.Verify(sp => sp.GetService(typeof(BaseSubscriber)), Times.Once);
        Assert.Equal("No service for type '" + typeof(BaseSubscriber).FullName + "' has been registered."
            , ex.Message);
    }

    [Fact]
    public async Task Delegate_Twice_Success()
    {
        var handlerMock = new Mock<Func<Event1, IServiceProvider, CancellationToken, Task>>();
        var options = new EventAggregatorOptions();
        options.AddHandler1(handlerMock.Object);
        options.AddHandler1(handlerMock.Object);
        var service = new EACore(null, options);

        await service.PublishAsync(new Event1());

        handlerMock.Verify(d => d(It.IsAny<Event1>(), null, It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task AsyncSubscriberForm6_Thrice_Success()
    {
        var handlerMock = new Mock<BaseAsyncSubscriber>();
        var spMock = new Mock<IServiceProvider>();
        spMock.Setup(sp => sp.GetService(typeof(BaseAsyncSubscriber))).Returns(handlerMock.Object).Verifiable();

        var options = new EventAggregatorOptions();
        options.AddHandler6<Event1, BaseAsyncSubscriber>();
        options.AddHandler6<Event1, BaseAsyncSubscriber>();
        options.AddHandler6<Event1, BaseAsyncSubscriber>();
        var service = new EACore(spMock.Object, options);

        await service.PublishAsync(new Event1());

        handlerMock.Verify(s => s.HandleAsync(It.IsAny<Event1>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        spMock.VerifyAll();
        spMock.Verify(sp => sp.GetService(typeof(BaseAsyncSubscriber)), Times.Exactly(3));
    }

    [Fact]
    public async Task AllForm_Success()
    {
        var options = new EventAggregatorOptions();
        var spMock = new Mock<IServiceProvider>();
        var sp = spMock.Object;

        var handler1Mock = new Mock<Func<Event1, IServiceProvider, CancellationToken, Task>>();
        options.AddHandler1(handler1Mock.Object);

        var handler2Mock = new Mock<Action<Event1, IServiceProvider>>();
        options.AddHandler2(handler2Mock.Object);

        var handler3Mock = new Mock<Func<Event1, IServiceProvider, Task>>();
        options.AddHandler3(handler3Mock.Object);

        var handler4Mock = new Mock<Func<Event1, Task>>();
        options.AddHandler4(handler4Mock.Object);

        var handler5Mock = new Mock<Action<Event1>>();
        options.AddHandler5(handler5Mock.Object);

        var handler6Mock = new Mock<BaseAsyncSubscriber>();
        spMock.Setup(sp => sp.GetService(typeof(BaseAsyncSubscriber))).Returns(handler6Mock.Object).Verifiable();
        options.AddHandler6<Event1, BaseAsyncSubscriber>();

        var handler7Mock = new Mock<BaseSubscriber>();
        spMock.Setup(sp => sp.GetService(typeof(BaseSubscriber))).Returns(handler7Mock.Object).Verifiable();
        options.AddHandler7<Event1, BaseSubscriber>();

        var service = new EACore(sp, options);

        await service.PublishAsync(new Event1());

        handler1Mock.Verify(d => d(It.IsAny<Event1>(), sp, It.IsAny<CancellationToken>()), Times.Once);
        handler2Mock.Verify(d => d(It.IsAny<Event1>(), sp), Times.Once);
        handler3Mock.Verify(d => d(It.IsAny<Event1>(), sp), Times.Once);
        handler4Mock.Verify(d => d(It.IsAny<Event1>()), Times.Once);
        handler5Mock.Verify(d => d(It.IsAny<Event1>()), Times.Once);
        handler6Mock.Verify(s => s.HandleAsync(It.IsAny<Event1>(), It.IsAny<CancellationToken>()), Times.Once);
        handler7Mock.Verify(s => s.Handle(It.IsAny<Event1>()), Times.Once);

        spMock.VerifyAll();
        spMock.Verify(sp => sp.GetService(typeof(BaseAsyncSubscriber)), Times.Once);
        spMock.Verify(sp => sp.GetService(typeof(BaseSubscriber)), Times.Once);

        await service.PublishAsync(new Event1());
        await service.PublishAsync(new Event1());

        handler1Mock.Verify(d => d(It.IsAny<Event1>(), sp, It.IsAny<CancellationToken>()), Times.Exactly(3));
        handler2Mock.Verify(d => d(It.IsAny<Event1>(), sp), Times.Exactly(3));
        handler3Mock.Verify(d => d(It.IsAny<Event1>(), sp), Times.Exactly(3));
        handler4Mock.Verify(d => d(It.IsAny<Event1>()), Times.Exactly(3));
        handler5Mock.Verify(d => d(It.IsAny<Event1>()), Times.Exactly(3));
        handler6Mock.Verify(s => s.HandleAsync(It.IsAny<Event1>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        handler7Mock.Verify(s => s.Handle(It.IsAny<Event1>()), Times.Exactly(3));
        spMock.Verify(sp => sp.GetService(typeof(BaseAsyncSubscriber)), Times.Exactly(3));
        spMock.Verify(sp => sp.GetService(typeof(BaseSubscriber)), Times.Exactly(3));
    }
}
