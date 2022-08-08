using Microsoft.AspNetCore.Mvc;
using NetLah.Extensions.EventAggregator;
using SampleWebApi.Models;
using SampleWebApi.Services;

namespace SampleWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IEventAggregator _ea;
    private readonly IRootEventAggregator _rootEa;
    private readonly TransientService1 _transientService1;
    private readonly SingletonService1 _singletonService1;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IEventAggregator ea, IRootEventAggregator rootEa,
        TransientService1 transientService1, SingletonService1 singletonService1)
    {
        _logger = logger;
        _ea = ea;
        _rootEa = rootEa;
        _transientService1 = transientService1;
        _singletonService1 = singletonService1;
    }

    private static WeatherForecast[] Fetch(int count)
    {
        var rng = new Random();
        var result = Enumerable.Range(1, count).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = rng.Next(-20, 55),
            Summary = Summaries[rng.Next(Summaries.Length)]
        })
        .ToArray();
        return result;
    }

    [HttpGet]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        var result = Fetch(5);

        await Publish("[Publish1]", result.First(), msg => new Event1 { Message = msg }, _ea);
        await Publish("[Publish2]", result.Skip(1).First(), msg => new Event2 { Message2 = msg }, _ea);
        await Publish("[Publish3a]", result.Skip(2).First(), msg => new Event3A { Message3 = msg }, _ea);
        await Publish("[Publish3b]", result.Skip(3).First(), msg => new Event3B { Message3 = msg }, _ea);

        return result;
    }

    [HttpGet("Root")]
    public async Task<IEnumerable<WeatherForecast>> Head()
    {
        var result = Fetch(1);

        await PublishAllToRoot("[PublishRoot]", result.First(), _rootEa);

        return result;
    }

    [HttpGet("Transient")]
    public async Task<IEnumerable<WeatherForecast>> TransientGet()
    {
        var result = Fetch(1);
        var data = result.First();
        var message = $"{data.Date} {data.TemperatureC} {data.TemperatureF} {data.Summary}";

        await _transientService1.SaveMessageAsync(message);

        return result;
    }

    [HttpGet("Singleton")]
    public async Task<IEnumerable<WeatherForecast>> GetSingleton()
    {
        var result = Fetch(1);
        var data = result.First();
        var message = $"{data.Date} {data.TemperatureC} {data.TemperatureF} {data.Summary}";

        await _singletonService1.SendMessageAsync(message);

        return result;
    }

    private Task Publish<TEvent>(string header, WeatherForecast data, Func<string, TEvent> factory, IEventAggregator ea) where TEvent : IEvent
    {
        var message = $"{data.Date} {data.TemperatureC} {data.TemperatureF} {data.Summary}";
        _logger.LogInformation(header + " {message}", message);
        return ea.PublishAsync(factory(message));
    }

    private async Task PublishAllToRoot(string header, WeatherForecast @event, IEventAggregator ea)
    {
        var message = $"{@event.Date} {@event.TemperatureC} {@event.TemperatureF} {@event.Summary}";
        _logger.LogInformation(header + " {message}", message);
        await ea.PublishAsync(new Event1 { Message = message });
        await ea.PublishAsync(new Event2 { Message2 = message });
        await ea.PublishAsync(new Event3A { Message3 = message });
        await ea.PublishAsync(new Event3B { Message3 = message });
    }
}
