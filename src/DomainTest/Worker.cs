using Domain;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace DomainTestCore
{
    internal class Worker : IHostedService
    {
        private readonly IHouse _house;
        public Worker(IHouse house)
        {
            _house = house;
        }

        public  Task StartAsync(CancellationToken cancellationToken)
        {
            _house.Start();
            return Task.CompletedTask;
        }
        
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _house.Stop();
            return Task.CompletedTask;
        }
    }
}