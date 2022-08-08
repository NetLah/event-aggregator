using NetLah.Extensions.EventAggregator;
using SampleWebApi.Models;

namespace SampleWebApi.Services;

public class SingletonService1
{
    private readonly ILogger _logger;
    private readonly IRootEventAggregator _rootEa;
    private readonly TransientService2 _transientService2;

    public SingletonService1(ILogger<SingletonService1> logger, IRootEventAggregator rootEa, TransientService2 transientService2)    //, TransientService1 transientService1)
    {
        _logger = logger;
        _rootEa = rootEa;
        _transientService2 = transientService2;
    }

    public async Task SendMessageAsync(string message)
    {
        _logger.LogInformation("[SingletonService1] {message}", message);
        await _rootEa.PublishAsync(new Event1 { Message = message });
        await _transientService2.SaveMessageAsync(message);
    }
}
