using log4net;
using MemBus;
using MemBus.Configurators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain
{
    public class VirtualHouse : IHouse
    {
        private readonly IBus _bus = BusSetup.StartWith<Conservative>().Construct();
        private static readonly ILog Log = LogManager.GetLogger(typeof(VirtualHouse));
        private readonly DeviceList _devices = new DeviceList();
        private readonly IList<IService> _services = new List<IService>();

        public VirtualHouse()
        {
            //TODO: initialize from settings file
            _services.Add(new EasywaveDeviceManager(_bus, _devices));
            _devices.Add(new EldatRx09Transceiver("COM3", _bus));
            _devices.Add(new EasywaveButton(2258148, "Keuken1"));
            _devices.Add(new EasywaveButton(2267862, "Keuken2"));
            _devices.Add(new EasywaveButton(2270401, "Keuken3"));
            _devices.Add(new SimpleReceiver("Gootsteen", _bus, new Subscription(2258148, KeyCode.A), new Subscription(16, KeyCode.A, true)));
            _devices.Add(new SimpleReceiver("KeukenTafel", _bus, new Subscription(2258148, KeyCode.C), new Subscription(2270401, KeyCode.A)));
            var lamp = new Lamp("Gootsteen Keuken", _bus, "Gootsteen");
            _devices.Add(lamp);
            Task.Delay(10000).ContinueWith((t) => lamp.TurnOnAsync());
        }

        public void Stop()
        {
            Log.Warn("Stopping...");
            Log.Debug("Stopping devices...");
            foreach(IDevice device in _devices)
            {
                if (device is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            Log.Debug("Stopping services...");
            foreach(IService service in _services)
            {
                if (service is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        public void Start()
        {
            Log.Warn("Starting...");
        }

        internal class DeviceList :  IDeviceList
        {
            private readonly List<IDevice> _list = new List<IDevice>();
            public void Add(IDevice device)
            {
                _list.Add(device);
            }

            public void Remove(IDevice device)
            {
                _list.Remove(device);
            }

            public IEnumerator<IDevice> GetEnumerator()
            {
                return _list.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

     }
}
