using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace NetLah.Extensions.EventAggregator.Test
{
    public class DependencyInjectionTest
    {
        private static ServiceCollection GetServices()
        {
            var services = new ServiceCollection();
            services.AddOptions();
            services.AddEventAggregator();
            return services;
        }

        [Fact]
        public async Task AddEventAggregator_Success()
        {
            var services = new ServiceCollection();
            Assert.Empty(services);

            services.AddOptions();
            var optionsServicesCount = services.Count;

            services.AddEventAggregator();
            Assert.Equal(optionsServicesCount + 2, services.Count);

            services.AddEventAggregator();
            Assert.Equal(optionsServicesCount + 2, services.Count);

            await using var serviceProvider = services.BuildServiceProvider(validateScopes: true);

            var root = serviceProvider.GetRequiredService<IRootEventAggregator>();
            Assert.NotNull(root);

            Assert.Throws<InvalidOperationException>(() => serviceProvider.GetRequiredService<IEventAggregator>());

            await using var scope = new AsyncDisposableWrapper<IServiceScope>(serviceProvider.CreateScope());

            var root2 = scope.Service.ServiceProvider.GetRequiredService<IRootEventAggregator>();
            Assert.Same(root, root2);

            var instance = scope.Service.ServiceProvider.GetRequiredService<IEventAggregator>();
            Assert.NotNull(instance);
        }

        [Fact]
        public async Task ResolveTransient_Success()
        {
            var services = new ServiceCollection();

            services.AddOptions();
            services.AddEventAggregator();
            services.AddTransient<TransientTestService1>();

            await using var serviceProvider = services.BuildServiceProvider(validateScopes: true);

            Assert.Throws<InvalidOperationException>(() => serviceProvider.GetRequiredService<TransientTestService1>());

            await using var scope = new AsyncDisposableWrapper<IServiceScope>(serviceProvider.CreateScope());

            var scopeEa = scope.Service.ServiceProvider.GetRequiredService<IEventAggregator>();
            Assert.NotNull(scopeEa);

            var instance = scope.Service.ServiceProvider.GetRequiredService<TransientTestService1>();
            Assert.NotNull(instance);

            Assert.Same(scopeEa, instance.EventAggregator);
        }

        [Fact]
        public async Task ScopedDelegateForm1Async_Success()
        {
            var services = GetServices();

            var handlerMock = new Mock<Handler1Async<TestEvent1>>();
            _ = services.SubscribeAsync<TestEvent1>(handlerMock.Object.Invoke);

            await using var serviceProvider = services.BuildServiceProvider(validateScopes: true);
            using var scope = serviceProvider.CreateScope();

            var ea = scope.ServiceProvider.GetRequiredService<IEventAggregator>();
            IEvent @event = new TestEvent1();
            await ea.PublishAsync(@event);

            handlerMock.Verify(d => d((TestEvent1)@event, scope.ServiceProvider, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ScopedDelegateForm3Async_Success()
        {
            var services = GetServices();

            var handlerMock = new Mock<Handler3Async<TestEvent1>>();
            _ = services.SubscribeAsync<TestEvent1>(handlerMock.Object.Invoke);

            await using var serviceProvider = services.BuildServiceProvider(validateScopes: true);
            using var scope = serviceProvider.CreateScope();

            var ea = scope.ServiceProvider.GetRequiredService<IEventAggregator>();
            IEvent @event = new TestEvent1();
            await ea.PublishAsync(@event);

            handlerMock.Verify(d => d((TestEvent1)@event, scope.ServiceProvider), Times.Once);
        }

        [Fact]
        public async Task ScopedIAsyncSubscriber_Success()
        {
            var services = GetServices();

            var handlerMock = new Mock<TestAsyncSubscriber>();
            services.AddScoped<TestAsyncSubscriber>(sp => handlerMock.Object);
            _ = services.SubscribeAsync<TestEvent1, TestAsyncSubscriber>();

            await using var serviceProvider = services.BuildServiceProvider(validateScopes: true);
            using var scope = serviceProvider.CreateScope();

            var ea = scope.ServiceProvider.GetRequiredService<IEventAggregator>();
            IEvent @event = new TestEvent1();
            await ea.PublishAsync(@event);

            handlerMock.Verify(s => s.HandleAsync((TestEvent1)@event, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ScopedDelegateForm2_Success()
        {
            var services = GetServices();

            var handlerMock = new Mock<Handler2<TestEvent1>>();
            _ = services.Subscribe<TestEvent1>(handlerMock.Object.Invoke);

            await using var serviceProvider = services.BuildServiceProvider(validateScopes: true);
            using var scope = serviceProvider.CreateScope();

            var ea = scope.ServiceProvider.GetRequiredService<IEventAggregator>();
            IEvent @event = new TestEvent1();
            await ea.PublishAsync(@event);

            handlerMock.Verify(d => d((TestEvent1)@event, scope.ServiceProvider), Times.Once);
        }

        [Fact]
        public async Task ScopedISubscriber_Success()
        {
            var services = GetServices();

            var handlerMock = new Mock<TestSubscriber>();
            services.AddScoped<TestSubscriber>(sp => handlerMock.Object);
            _ = services.Subscribe<TestEvent1, TestSubscriber>();

            await using var serviceProvider = services.BuildServiceProvider(validateScopes: true);
            using var scope = serviceProvider.CreateScope();

            var ea = scope.ServiceProvider.GetRequiredService<IEventAggregator>();
            IEvent @event = new TestEvent1();
            await ea.PublishAsync(@event);

            handlerMock.Verify(s => s.Handle((TestEvent1)@event), Times.Once);
        }

        [Fact]
        public async Task SingletonScopedDelegateForm1Async_Success()
        {
            var services = GetServices();

            var handlerMock = new Mock<Handler1Async<TestEvent1>>();
            _ = services.SubscribeAsync<TestEvent1>(handlerMock.Object.Invoke, SubscriberLifetime.Singleton);

            await using var serviceProvider = services.BuildServiceProvider(validateScopes: true);
            using var scope = serviceProvider.CreateScope();

            var ea = scope.ServiceProvider.GetRequiredService<IEventAggregator>();
            IEvent @event = new TestEvent1();
            await ea.PublishAsync(@event);

            handlerMock.Verify(d => d((TestEvent1)@event, scope.ServiceProvider, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SingletonScopedDelegateForm3Async_Success()
        {
            var services = GetServices();

            var handlerMock = new Mock<Handler3Async<TestEvent1>>();
            _ = services.SubscribeAsync<TestEvent1>(handlerMock.Object.Invoke, SubscriberLifetime.Singleton);

            await using var serviceProvider = services.BuildServiceProvider(validateScopes: true);
            using var scope = serviceProvider.CreateScope();

            var ea = scope.ServiceProvider.GetRequiredService<IEventAggregator>();
            IEvent @event = new TestEvent1();
            await ea.PublishAsync(@event);

            handlerMock.Verify(d => d((TestEvent1)@event, scope.ServiceProvider), Times.Once);
        }

        [Fact]
        public async Task SingletonScopedIAsyncSubscriber_Success()
        {
            var services = GetServices();

            var handlerMock = new Mock<TestAsyncSubscriber>();
            services.AddSingleton<TestAsyncSubscriber>(sp => handlerMock.Object);
            _ = services.SubscribeAsync<TestEvent1, TestAsyncSubscriber>(SubscriberLifetime.Singleton);

            await using var serviceProvider = services.BuildServiceProvider(validateScopes: true);
            using var scope = serviceProvider.CreateScope();

            var ea = scope.ServiceProvider.GetRequiredService<IEventAggregator>();
            IEvent @event = new TestEvent1();
            await ea.PublishAsync(@event);

            handlerMock.Verify(s => s.HandleAsync((TestEvent1)@event, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SingletonScopedDelegateForm2_Success()
        {
            var services = GetServices();

            var handlerMock = new Mock<Handler2<TestEvent1>>();
            _ = services.Subscribe<TestEvent1>(handlerMock.Object.Invoke, SubscriberLifetime.Singleton);

            await using var serviceProvider = services.BuildServiceProvider(validateScopes: true);
            using var scope = serviceProvider.CreateScope();

            var ea = scope.ServiceProvider.GetRequiredService<IEventAggregator>();
            IEvent @event = new TestEvent1();
            await ea.PublishAsync(@event);

            handlerMock.Verify(d => d((TestEvent1)@event, scope.ServiceProvider), Times.Once);
        }

        [Fact]
        public async Task SingletonScopedISubscriber_Success()
        {
            var services = GetServices();

            var handlerMock = new Mock<TestSubscriber>();
            services.AddSingleton<TestSubscriber>(sp => handlerMock.Object);
            _ = services.Subscribe<TestEvent1, TestSubscriber>(SubscriberLifetime.Singleton);

            await using var serviceProvider = services.BuildServiceProvider(validateScopes: true);
            using var scope = serviceProvider.CreateScope();

            var ea = scope.ServiceProvider.GetRequiredService<IEventAggregator>();
            IEvent @event = new TestEvent1();
            await ea.PublishAsync(@event);

            handlerMock.Verify(s => s.Handle((TestEvent1)@event), Times.Once);
        }

        [Fact]
        public async Task SingletonDelegateForm1Async_Success()
        {
            var services = GetServices();

            var handlerMock = new Mock<Handler1Async<TestEvent1>>();
            _ = services.SubscribeAsync<TestEvent1>(handlerMock.Object.Invoke, SubscriberLifetime.Singleton);

            await using var serviceProvider = services.BuildServiceProvider(validateScopes: true);

            var sp = serviceProvider.GetRequiredService<IServiceProvider>();
            var rootEA = serviceProvider.GetRequiredService<IRootEventAggregator>();
            IEvent @event = new TestEvent1();
            await rootEA.PublishAsync(@event);

            handlerMock.Verify(d => d((TestEvent1)@event, sp, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SingletonDelegateForm3Async_Success()
        {
            var services = GetServices();

            var handlerMock = new Mock<Handler3Async<TestEvent1>>();
            _ = services.SubscribeAsync<TestEvent1>(handlerMock.Object.Invoke, SubscriberLifetime.Singleton);

            await using var serviceProvider = services.BuildServiceProvider(validateScopes: true);

            var sp = serviceProvider.GetRequiredService<IServiceProvider>();
            var rootEA = serviceProvider.GetRequiredService<IRootEventAggregator>();
            IEvent @event = new TestEvent1();
            await rootEA.PublishAsync(@event);

            handlerMock.Verify(d => d((TestEvent1)@event, sp), Times.Once);
        }

        [Fact]
        public async Task SingletonIAsyncSubscriber_Success()
        {
            var services = GetServices();

            var handlerMock = new Mock<TestAsyncSubscriber>();
            services.AddSingleton<TestAsyncSubscriber>(sp => handlerMock.Object);
            _ = services.SubscribeAsync<TestEvent1, TestAsyncSubscriber>(SubscriberLifetime.Singleton);

            await using var serviceProvider = services.BuildServiceProvider(validateScopes: true);

            var rootEA = serviceProvider.GetRequiredService<IRootEventAggregator>();
            IEvent @event = new TestEvent1();
            await rootEA.PublishAsync(@event);

            handlerMock.Verify(s => s.HandleAsync((TestEvent1)@event, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SingletonDelegateForm2_Success()
        {
            var services = GetServices();

            var handlerMock = new Mock<Handler2<TestEvent1>>();
            _ = services.Subscribe<TestEvent1>(handlerMock.Object.Invoke, SubscriberLifetime.Singleton);

            await using var serviceProvider = services.BuildServiceProvider(validateScopes: true);

            var sp = serviceProvider.GetRequiredService<IServiceProvider>();
            var rootEA = serviceProvider.GetRequiredService<IRootEventAggregator>();
            IEvent @event = new TestEvent1();
            await rootEA.PublishAsync(@event);

            handlerMock.Verify(d => d((TestEvent1)@event, sp), Times.Once);
        }

        [Fact]
        public async Task SingletonISubscriber_Success()
        {
            var services = GetServices();

            var handlerMock = new Mock<TestSubscriber>();
            services.AddSingleton<TestSubscriber>(sp => handlerMock.Object);
            _ = services.Subscribe<TestEvent1, TestSubscriber>(SubscriberLifetime.Singleton);

            await using var serviceProvider = services.BuildServiceProvider(validateScopes: true);

            var rootEA = serviceProvider.GetRequiredService<IRootEventAggregator>();
            IEvent @event = new TestEvent1();
            await rootEA.PublishAsync(@event);

            handlerMock.Verify(s => s.Handle((TestEvent1)@event), Times.Once);
        }

        [Fact]
        public async Task SingletonThenScoped_Success()
        {
            var services = GetServices();

            var handler1Mock = new Mock<TestAsyncSubscriber>();
            services.AddSingleton<TestAsyncSubscriber>(sp => handler1Mock.Object);

            var handler2Mock = new Mock<Test2AsyncSubscriber>();
            services.AddSingleton<Test2AsyncSubscriber>(sp => handler2Mock.Object);

            _ = services.SubscribeAsync<TestEvent1, TestAsyncSubscriber>(SubscriberLifetime.Singleton);
            _ = services.SubscribeAsync<TestEvent1, Test2AsyncSubscriber>(SubscriberLifetime.Scoped);

            await using var serviceProvider = services.BuildServiceProvider(validateScopes: true);

            var rootEA = serviceProvider.GetRequiredService<IRootEventAggregator>();
            IEvent event1 = new TestEvent1();
            await rootEA.PublishAsync(event1);
            handler1Mock.Verify(s => s.HandleAsync((TestEvent1)event1, It.IsAny<CancellationToken>()), Times.Once);
            handler2Mock.Verify(s => s.HandleAsync((TestEvent1)event1, It.IsAny<CancellationToken>()), Times.Never);

            using var scope = serviceProvider.CreateScope();

            var ea = scope.ServiceProvider.GetRequiredService<IEventAggregator>();
            IEvent event2 = new TestEvent1();
            await ea.PublishAsync(event2);

            handler1Mock.Verify(s => s.HandleAsync((TestEvent1)event2, It.IsAny<CancellationToken>()), Times.Once);
            handler2Mock.Verify(s => s.HandleAsync((TestEvent1)event2, It.IsAny<CancellationToken>()), Times.Once);

            handler1Mock.Verify(s => s.HandleAsync(It.IsAny<TestEvent1>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            handler2Mock.Verify(s => s.HandleAsync(It.IsAny<TestEvent1>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        private class TransientTestService1
        {
#pragma warning disable S1144 // Unused private types or members should be removed
            public TransientTestService1(IEventAggregator ea)
            {
                EventAggregator = ea;
            }
#pragma warning restore S1144 // Unused private types or members should be removed

            public IEventAggregator EventAggregator { get; }
        }

        internal class TestEvent1 : IEvent { }

        internal abstract class TestAsyncSubscriber : IAsyncSubscriber<TestEvent1>
        {
            public abstract Task HandleAsync(TestEvent1 @event, CancellationToken cancellationToken = default);
        }

        internal abstract class Test2AsyncSubscriber : IAsyncSubscriber<TestEvent1>
        {
            public abstract Task HandleAsync(TestEvent1 @event, CancellationToken cancellationToken = default);
        }

        internal abstract class TestSubscriber : ISubscriber<TestEvent1>
        {
            public abstract void Handle(TestEvent1 @event);
        }
    }
}
