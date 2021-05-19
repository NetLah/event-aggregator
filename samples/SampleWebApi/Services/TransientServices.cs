using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NetLah.Extensions.EventAggregator;
using SampleWebApi.Models;

namespace SampleWebApi.Services
{
    public class TransientService1
    {
        private readonly ILogger _logger;
        private readonly IEventAggregator _ea;

        public TransientService1(ILogger<TransientService1> logger, IEventAggregator ea)
        {
            _logger = logger;
            _ea = ea;
        }

        public Task SaveMessageAsync(string message)
        {
            _logger.LogInformation("[TransientService1] {message}", message);
            return _ea.PublishAsync(new Event1 { Message = message });
        }
    }

    public class TransientService2
    {
        private readonly ILogger _logger;
        private readonly IEventAggregator _rootEA;

        public TransientService2(ILogger<TransientService2> logger, IRootEventAggregator rootEA)
        {
            _logger = logger;
            _rootEA = rootEA;
        }

        public Task SaveMessageAsync(string message)
        {
            _logger.LogInformation("[TransientService2] {message}", message);
            return _rootEA.PublishAsync(new Event2 { Message2 = message });
        }
    }
}
