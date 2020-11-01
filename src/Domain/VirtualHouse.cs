using log4net;
using MemBus;
using System;

namespace Domain
{
    public class VirtualHouse : IHouse
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(VirtualHouse));
        private readonly ICanStartAndStopList<IDevice> _devices;
        private readonly ICanStartAndStopList<IService> _services;

        public VirtualHouse(ICanStartAndStopList<IDevice> devices, ICanStartAndStopList<IService> services, IBus bus)
        {
            _devices = devices;
            _services = services;
            //TODO: initialize from settings file
            var kitchenSink = new EasywaveReceiver("KitchenSink Receiver", bus, new Subscription(2258148, KeyCode.A), new Subscription(16, KeyCode.A, true));
            var kitchenTable = new EasywaveReceiver("KitchenTable Receiver", bus, new Subscription(2258148, KeyCode.C), new Subscription(2270401, KeyCode.A));
            var terrace = new EasywaveReceiver("Terrace Receiver", bus, new Subscription(2267862, KeyCode.A));
            var hall = new EasywaveReceiver("Hall Receiver", bus);
            var nightHall = new EasywaveReceiver("NightHall Receiver", bus);
            var laundryRoom = new EasywaveReceiver("LaundryRoom Receiver", bus, new Subscription(2266558, KeyCode.A));
            var bathRoom = new EasywaveReceiver("BathRoom Receiver", bus);
            var masterBedRoom = new EasywaveReceiver("MasterBedRoom Receiver", bus);
            var dressing = new EasywaveReceiver("Dressing Receiver", bus);
            _devices.Add(kitchenTable);
            _devices.Add(kitchenSink);
            _devices.Add(terrace);
            _devices.Add(hall);
            _devices.Add(nightHall);
            _devices.Add(laundryRoom);
            _devices.Add(bathRoom);
            _devices.Add(masterBedRoom);
            _devices.Add(dressing);
            _devices.Add(new EasywaveButton(2258148, "Kitchen1"));
            _devices.Add(new EasywaveButton(2267862, "Kitchen2"));
            _devices.Add(new EasywaveButton(2270401, "Kitchen3"));
            _devices.Add(new EasywaveButton(2266558, "LaundryRoom1"));
            _devices.Add(new Lamp("KitchenSink Light", kitchenSink.Name,bus));
            _devices.Add(new Lamp("KitchenTable Light", kitchenTable.Name,bus));
            _devices.Add(new Lamp("Terrace Light", terrace.Name,bus));
            _devices.Add(new Lamp("Laundry room", laundryRoom.Name, bus));
            _services.Add(new EasywaveDeviceManager(bus, _devices));
            _services.Add(new EldatRx09Transceiver("COM3", bus));

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
