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

[assembly: XmlConfigurator(ConfigFile = "log4net.config")]

namespace DomainTestCore
{
    internal class Program
    {
        private static ServiceList<IService> _services;
        private static DeviceList<IDevice> _devices;

        private static void Main(string[] args)
        {
            try
            {
                _services = new ServiceList<IService>();
                _devices = new DeviceList<IDevice>();
                CreateHostBuilder(args).Build().Run();
                Save();
            }
            catch (OperationCanceledException)
            {
                //ignore
            }
        }

        private static void Save()
        {
            File.WriteAllText("C:\\Temp\\Devices.json", JsonConvert.SerializeObject(_devices));
            File.WriteAllText("C:\\Temp\\Services.json", JsonConvert.SerializeObject(_services));
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(b => b.AddLog4Net(false))
                .ConfigureServices((hostContext, services) =>
                    {
                        //Configure the services needed to run everything
                        services.AddSingleton(BusSetup.StartWith<Conservative>().Construct());
                        services.AddSingleton<ICanStartAndStopList<IService>>(_services);
                        services.AddSingleton<ICanStartAndStopList<IDevice>>(_devices);
                        services.AddSingleton<IHouse, VirtualHouse>();
                        //Add the worker service
                        services.AddHostedService<Worker>();
                    })
                .UseConsoleLifetime();
        }

    }

}
