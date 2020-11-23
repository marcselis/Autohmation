using Domain;
using Domain.Services;
using log4net.Config;
using MemBus;
using MemBus.Configurators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Xml.Serialization;

[assembly: XmlConfigurator(ConfigFile = "log4net.config")]

namespace DomainTestCore
{
    internal class Program
    {

        private static void Main(string[] args)
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
                        //Configure the services needed to run everything
                        services.AddSingleton(BusSetup.StartWith<Conservative>().Construct());
                        services.AddSingleton<IHouse, VirtualHouse>();
                        //Add the worker service
                        services.AddHostedService<Worker>();
                    })
                .UseConsoleLifetime();
        }

    }

}
