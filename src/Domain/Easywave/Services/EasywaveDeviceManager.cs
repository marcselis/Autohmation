using log4net;
using MemBus;
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Domain
{
    public class EasywaveDeviceManager : AutohmationService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(EasywaveDeviceManager));
        private readonly ICanStartAndStopList<IDevice> _devices;
        private readonly IBus _bus;
        private IDisposable _subscription;

        public EasywaveDeviceManager(IBus bus, ICanStartAndStopList<IDevice> devices) 
        {
            _bus = bus;
            _devices = devices;
        }

        public override void Start()
        {
            Log.Debug("EasywaveDeviceManager is starting...");
            _subscription = _bus.Subscribe((EasywaveTelegram telegram) => CheckEasywaveButtonExists(telegram));
        }

        public override void Stop()
        {
            Log.Debug("EasywaveDeviceManager is stopping...");
            _subscription?.Dispose();
            _subscription = null;
        }

        private void CheckEasywaveButtonExists(EasywaveTelegram telegram)
        {
            var button = (IEasywaveButton)_devices.FirstOrDefault(d => typeof(IEasywaveButton).IsAssignableFrom(d.GetType()) && ((IEasywaveButton)d).Address == telegram.Address);
            if (button == null)
            {
                Log.Info($"Detected new EasywaveButton at {telegram.Address}");
                EasywaveButton device = new EasywaveButton(telegram.Address);
                _devices.Add(device);
                _bus.Publish(new DeviceAdded(device));
            }
        }


    }
}
