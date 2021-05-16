using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetLah.Extensions.EventAggregator;
using SampleWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleWebApi.Controllers
{
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

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IEventAggregator ea, IRootEventAggregator rootEa)
        {
            _logger = logger;
            _ea = ea;
            _rootEa = rootEa;
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

        [HttpHead]
        public async Task<IEnumerable<WeatherForecast>> Head()
        {
            var result = Fetch(1);

            await PublishAllToRoot("[PublishRoot]", result.First(), _rootEa);

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
}
