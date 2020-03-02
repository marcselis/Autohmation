using log4net;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Domain
{
    public class VirtualHouse : IHouse
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(VirtualHouse));
        private readonly ICanStartAndStopList<IDevice> _devices;
        private readonly ICanStartAndStopList<IService> _services;

        public VirtualHouse(IServiceProvider services)
        {
            _devices = services.GetService<ICanStartAndStopList<IDevice>>();
            _services = services.GetService<ICanStartAndStopList<IService>>();
            //TODO: initialize from settings file
            _services.Add(new EasywaveDeviceManager(services));
            _services.Add(new EldatRx09Transceiver("COM3", services));
            _devices.Add(new EasywaveButton(2258148, "Keuken1"));
            _devices.Add(new EasywaveButton(2267862, "Keuken2"));
            _devices.Add(new EasywaveButton(2270401, "Keuken3"));
            _devices.Add(new EasywaveReceiver("Gootsteen", services, new Subscription(2258148, KeyCode.A), new Subscription(16, KeyCode.A, true)));
            _devices.Add(new EasywaveReceiver("KeukenTafel", services, new Subscription(2258148, KeyCode.C), new Subscription(2270401, KeyCode.A)));
            Lamp lamp = new Lamp("Gootsteen Keuken", services, "Gootsteen");
            _devices.Add(lamp);
            //Test turning a lamp on
            //Task.Delay(10000).ContinueWith((t) => lamp.TurnOnAsync());
        }

        public void Stop()
        {
            Log.Warn("Stopping...");
            Log.Debug("Stopping devices...");
            _devices.Stop();
            Log.Debug("Stopping services...");
            _services.Stop();
        }

        public void Start()
        {
            Log.Warn("Starting...");
            Log.Debug("Starting services...");
            _services.Start();
            Log.Debug("Starting devices...");
            _devices.Start();
        }


    }
}
