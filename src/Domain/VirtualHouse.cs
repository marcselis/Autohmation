using log4net;
using MemBus;

namespace Domain
{
    public class VirtualHouse : IHouse
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(VirtualHouse));

        public ICanStartAndStopList<IDevice> Devices { get; }

        public ICanStartAndStopList<IService> Services { get; }

        public VirtualHouse(ICanStartAndStopList<IDevice> devices, ICanStartAndStopList<IService> services, IBus bus)
        {
            Devices = devices;
            Services = services;
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
            Devices.Add(kitchenTable);
            Devices.Add(kitchenSink);
            Devices.Add(terrace);
            Devices.Add(hall);
            Devices.Add(nightHall);
            Devices.Add(laundryRoom);
            Devices.Add(bathRoom);
            Devices.Add(masterBedRoom);
            Devices.Add(dressing);
            Devices.Add(new EasywaveButton(2258148, "Kitchen1"));
            Devices.Add(new EasywaveButton(2267862, "Kitchen2"));
            Devices.Add(new EasywaveButton(2270401, "Kitchen3"));
            Devices.Add(new EasywaveButton(2266558, "LaundryRoom1"));
            Devices.Add(new Lamp("KitchenSink Light", kitchenSink.Name,bus));
            Devices.Add(new Lamp("KitchenTable Light", kitchenTable.Name,bus));
            Devices.Add(new Lamp("Terrace Light", terrace.Name,bus));
            Devices.Add(new Lamp("Laundry room", laundryRoom.Name, bus));
            Services.Add(new EasywaveDeviceManager(bus, Devices));
            Services.Add(new EldatRx09Transceiver("COM3", bus));

            //Test turning a lamp on
            //Task.Delay(10000).ContinueWith((t) => lamp.TurnOnAsync());
        }

        public void Stop()
        {
            Log.Warn("Stopping...");
            Log.Debug("Stopping devices...");
            Devices.Stop();
            Log.Debug("Stopping services...");
            Services.Stop();

        }

        public void Start()
        {
            Log.Warn("Starting...");
            Log.Debug("Starting services...");
            Services.Start();
            Log.Debug("Starting devices...");
            Devices.Start();
        }

  
    }
}
