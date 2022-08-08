using Moq;
using Xunit;

namespace NetLah.Extensions.EventAggregator.Test;

public class ThreadingTest
{
    private const int TaskCount = 10;
    private const long TotalIterations = 10_000_000_000;
#if DEBUG
    private const int TestDuration = 2_000;
#else
    private const int TestDuration = 20_000;
#endif

    private EACore _service;
    private CancellationTokenSource _cts;
    private long publish0;
    private long publish1;
    private long publish2;
    private long publish3;
    private long publish4;
    private long publish5;
    private long handled1;
    private long handled2;
    private long handled3;
    private long handled4;
    private long handled5;

    [Fact]
    public async Task MultiThreading_Success()
    {
        var options = new EventAggregatorOptions();
        Subscribe(options);
        var sp = Mock.Of<IServiceProvider>();
        _service = new EACore(sp, options);
        _cts = new CancellationTokenSource(TestDuration);

        var tasks = new List<Task>();
        for (int i = 0; i < TaskCount; i++)
        {
            tasks.Add(Task.Run(() => Runner(TotalIterations)));
        }

        await Task.WhenAll(tasks);

        Assert.True(publish0 > 0);
        Assert.True(publish1 > 0);
        Assert.True(publish2 > 0);
        Assert.True(publish3 > 0);
        Assert.True(publish4 > 0);
        Assert.True(publish5 > 0);

        Assert.Equal(handled1, publish1);
        Assert.Equal(handled2, publish2);
        Assert.Equal(handled3, publish3);
        Assert.Equal(handled4, publish4);
        Assert.Equal(handled5, publish5);
    }

    private void Subscribe(EventAggregatorOptions options)
    {
        options.AddHandler1<Event1>((e, sp, ct) => { NotNull(e); Interlocked.Increment(ref handled1); return Task.CompletedTask; });
        options.AddHandler2<Event2>((e, sp) => { NotNull(e); Interlocked.Increment(ref handled2); });
        options.AddHandler3<Event3>((e, sp) => { NotNull(e); Interlocked.Increment(ref handled3); return Task.CompletedTask; });
        options.AddHandler4<Event4>((e) => { NotNull(e); Interlocked.Increment(ref handled4); return Task.CompletedTask; });
        options.AddHandler5<Event5>((e) => { NotNull(e); Interlocked.Increment(ref handled5); });

        static void NotNull(IEvent e)
        {
#pragma warning disable S3928 // Parameter names used into ArgumentException constructors should match an existing one 
            if (e == null) throw new ArgumentNullException(nameof(e));
#pragma warning restore S3928 // Parameter names used into ArgumentException constructors should match an existing one 
        }
    }

    private async Task Runner(long total)
    {
        var rand = new Random();
        var ct = _cts.Token;

        for (long i = 0; i < total && !ct.IsCancellationRequested; i++)
        {
            var select = rand.Next(1000) % 7;

#pragma warning disable IDE0059 // Unnecessary assignment of a value
            IEvent e = null;
#pragma warning restore IDE0059 // Unnecessary assignment of a value

            switch (select)
            {
                case 0:
                    e = new Event0();
                    Interlocked.Increment(ref publish0);
                    break;

                case 1:
                    e = new Event1();
                    Interlocked.Increment(ref publish1);
                    break;

                case 2:
                    e = new Event2();
                    Interlocked.Increment(ref publish2);
                    break;

                case 3:
                    e = new Event3();
                    Interlocked.Increment(ref publish3);
                    break;

                case 4:
                    e = new Event4();
                    Interlocked.Increment(ref publish4);
                    break;

                case 5:
                    e = new Event5();
                    Interlocked.Increment(ref publish5);
                    break;

                case 6:
                    e = new Event1Child();
                    Interlocked.Increment(ref publish1);
                    break;

                default:
                    throw new InvalidOperationException("0-5");
            }

#pragma warning disable S3928 // Parameter names used into ArgumentException constructors should match an existing one 
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
#pragma warning disable S2583 // Conditionally executed code should be reachable
            await _service.PublishAsync(e ?? throw new ArgumentNullException("e"));
#pragma warning restore S2583 // Conditionally executed code should be reachable
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
#pragma warning restore S3928 // Parameter names used into ArgumentException constructors should match an existing one 
        }
    }
}
