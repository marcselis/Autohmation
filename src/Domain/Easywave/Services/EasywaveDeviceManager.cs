using log4net;
using MemBus;
using System;
using System.Linq;

namespace Domain
{
    public class EasywaveDeviceManager : AutohmationService
    {
        private readonly static ILog _log = LogManager.GetLogger(typeof(EasywaveDeviceManager));
        private readonly ICanStartAndStopList<IDevice> _devices;
        private readonly IBus _bus;
        private IDisposable? _subscription;

        public EasywaveDeviceManager(IBus bus, ICanStartAndStopList<IDevice> devices) 
        {
            _bus = bus;
            _devices = devices;
        }

        public override void Start()
        {
            _log.Debug("EasywaveDeviceManager is starting...");
            _subscription = _bus.Subscribe((EasywaveTelegram telegram) => CheckEasywaveButtonExists(telegram));
        }

        public override void Stop()
        {
            _log.Debug("EasywaveDeviceManager is stopping...");
            _subscription?.Dispose();
            _subscription = null;
        }

        private void CheckEasywaveButtonExists(EasywaveTelegram telegram)
        {
            var button = (IEasywaveButton?)_devices.FirstOrDefault(d => d is IEasywaveButton easywaveButton && easywaveButton.Address == telegram.Address);
            if (button != null)
            {
                return;
            }

            _log.Info($"Detected new EasywaveButton at {telegram.Address}");
            var device = new EasywaveButton(telegram.Address);
            _devices.Add(device);
            _bus.Publish(new DeviceAdded(device));
        }


    }
}
