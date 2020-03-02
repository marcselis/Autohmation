using Domain;
using log4net.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

[assembly: XmlConfigurator(ConfigFile = "log4net.config")]

namespace DomainTestCore
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (OperationCanceledException)
            {
                //ignore
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(b => b.AddLog4Net(false))
                .ConfigureServices((hostContext, services) =>
                    {
                        services.AddHostedService<Worker>();
                        services.AddSingleton<IHouse, VirtualHouse>();
                    });
        }

    }

}
