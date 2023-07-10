using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebsiteStatus
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private HttpClient _client;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _client = new HttpClient();
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _client.Dispose();
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                const string website = "https://www.raarge.com";
                var result = await _client.GetAsync(website, stoppingToken);

                if (result.IsSuccessStatusCode)
                {
                    _logger.LogInformation("The website was up. Status code {StatusCode} [{Website}]", result.StatusCode, website);
                }
                else
                {
                    _logger.LogError("The website is down. Status code {StatusCode} [{Website}]", result.StatusCode, website);
                }
                await Task.Delay(5 * 1000, stoppingToken);
            }
        }
    }
}
