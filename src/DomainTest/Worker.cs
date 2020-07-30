using Domain;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DomainTestCore
{
    internal class Worker : IHostedService
    {
        private readonly IHouse _house;
        private Task _loop;
        private CancellationTokenSource _cancellationSource;
        public Worker(IHouse house)
        {
            _house = house;
        }
    
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _house.Start();
            _cancellationSource = new CancellationTokenSource();
            _loop = Task.Run(() => MainLoop(_cancellationSource.Token));
            return Task.CompletedTask;
        }
        
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _house.Stop();
            _cancellationSource.Cancel();
            return _loop;
        }

        private void MainLoop(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (Console.KeyAvailable)
                {
                    var k = Console.ReadKey(true);
                    switch (k.KeyChar)
                    {
                        case 'g':
                            Console.WriteLine("GOGOGOOG");
                            break;
                        case 'q':
                            StopAsync(stoppingToken);
                            break;
                    }
                }
            }
        }
    }
}