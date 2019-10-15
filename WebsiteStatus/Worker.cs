using System;
using System.Collections.Generic;
using System.Linq;
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
        private HttpClient client;    // only open 1 client for the application otherwise can clog connections and crash computer

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            client = new HttpClient();   // starts when the service starts
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)  // call this when we stop the service
        {
            client.Dispose();   // cleaning the connections up and shut down the client
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var result = await client.GetAsync("https://www.raarge.com");    // make the call here to our website  Note: this will wait until the site loads then it will move to next counter
                
                if (result.IsSuccessStatusCode)
                {
                    _logger.LogInformation("The website was up. Status code {StatusCode}", result.StatusCode);
                }
                else
                {
                    _logger.LogError("The website is down. Status code {StatusCode}", result.StatusCode);   // this only a log
                    // here you would send an email to the person letting them know it is down.  (Perform an Action here)
                }
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);  //writing to log
                await Task.Delay(5 * 1000, stoppingToken);  // waiting 1 second and restarting the loop.   the task.delay is our control
            }
        }
    }
}
