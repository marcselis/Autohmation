using Domain;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DomainTestCore
{
  internal class Worker : IHostedService, IDisposable
  {
    private readonly IHouse _house;
    private readonly ILogger<Worker> _logger;
    private readonly IHostApplicationLifetime _lifetime;
    private Task? _loop;
    private CancellationTokenSource? _cancellationSource;
    private bool _disposedValue;

    public Worker(IHouse house, ILogger<Worker> logger, IHostApplicationLifetime lifetime)
    {
      _house = house;
      _logger = logger;
      _lifetime = lifetime;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      _logger.LogInformation("Starting worker");
      _house.Start();
      _cancellationSource = new CancellationTokenSource();
      _loop = Task.Run(() => MainLoop(_cancellationSource.Token), cancellationToken);
      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      _logger.LogInformation("Stopping worker");
      _house.Stop();
      _cancellationSource?.Cancel();
     // _lifetime.StopApplication();
      if (_loop != null)
      {
        return _loop;
      }
      return Task.CompletedTask;
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
              _lifetime.StopApplication();
              //await StopAsync(stoppingToken).ConfigureAwait(false);
              break;
          }
        }
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!_disposedValue)
      {
        if (disposing)
        {
          // TODO: dispose managed state (managed objects)
          _cancellationSource?.Dispose();
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        _disposedValue = true;
      }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~Worker()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }
  }
}