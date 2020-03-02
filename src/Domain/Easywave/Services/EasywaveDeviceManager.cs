using log4net;
using MemBus;
using System.Linq;

namespace Domain
{
    public class EasywaveDeviceManager : IService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(EasywaveDeviceManager));
        private readonly IDeviceList _devices;
        private readonly IBus _bus;

        public EasywaveDeviceManager(IBus bus, IDeviceList deviceList)
        {
            _bus = bus;
            _devices = deviceList;
            _bus.Subscribe((EasywaveTelegram telegram) => CheckEasywaveButtonExists(telegram));
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
