using Domain.Services;
using log4net;
using MemBus;
using System.IO;
using System.Xml.Serialization;

namespace Domain
{
  public class VirtualHouse : IHouse
  {
    private readonly static ILog _log = LogManager.GetLogger(typeof(VirtualHouse));

    private ICanStartAndStopList<IDevice> Devices { get; } = new DeviceList<IDevice>();

    private ICanStartAndStopList<IService> Services { get; } = new ServiceList<IService>();

    public VirtualHouse(IBus bus)
    {
      //TODO: initialize from settings file
      var kitchenSink = new EasywaveReceiver("KitchenSink Receiver", bus, new Subscription(2258148, KeyCode.A), new Subscription(16, KeyCode.A, true));
      var kitchenTable = new EasywaveReceiver("KitchenTable Receiver", bus, new Subscription(2258148, KeyCode.C), new Subscription(2270401, KeyCode.A));
      var terrace = new EasywaveReceiver("Terrace Receiver", bus, new Subscription(2267862, KeyCode.A));
      var hall = new EasywaveReceiver("Hall Receiver", bus, new Subscription(0x227e60, KeyCode.A));
      var nightHall = new EasywaveReceiver("NightHall Receiver", bus, new Subscription(0x227e60, KeyCode.C), new Subscription(0x229630, KeyCode.A), new Subscription(0x22963e, KeyCode.A), new Subscription(0x229964, KeyCode.A));
      var laundryRoom = new EasywaveReceiver("LaundryRoom Receiver", bus, new Subscription(2266558, KeyCode.A));
      var bathRoom = new EasywaveReceiver("BathRoom Receiver", bus, new Subscription(0x2295ce, KeyCode.A));
      var masterBedRoom = new EasywaveReceiver("MasterBedRoom Receiver", bus, new Subscription(0x229589, KeyCode.A), new Subscription(0x22759c, KeyCode.A));
      var dressing = new EasywaveReceiver("Dressing Receiver", bus, new Subscription(0x22759c, KeyCode.C));
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
      Devices.Add(new EasywaveButton(0x227e60, "Hall1"));
      Devices.Add(new EasywaveButton(0x2295ce, "Bathroom1"));
      Devices.Add(new EasywaveButton(0x229630, "NightHall1"));
      Devices.Add(new EasywaveButton(0x22963e, "NightHall2"));
      Devices.Add(new EasywaveButton(0x229964, "NightHall3"));
      Devices.Add(new EasywaveButton(0x229589, "MasterBedroom1"));
      Devices.Add(new EasywaveButton(0x22759c, "MasterBedroom2"));
      Devices.Add(new Lamp("KitchenSink Light", kitchenSink.Name, bus));
      Devices.Add(new Lamp("KitchenTable Light", kitchenTable.Name, bus));
      Devices.Add(new Lamp("Terrace Light", terrace.Name, bus));
      Devices.Add(new Lamp("Laundry Room Light", laundryRoom.Name, bus));
      Devices.Add(new Lamp("Hall Light", hall.Name, bus));
      Devices.Add(new Lamp("Bathroom Light", bathRoom.Name, bus));
      Devices.Add(new Lamp("NightHall Light", nightHall.Name, bus));
      Devices.Add(new Lamp("Master Bedroom Light", masterBedRoom.Name, bus));
      Devices.Add(new Lamp("Dressing Light", dressing.Name, bus));

      Services.Add(new EasywaveDeviceManager(bus, Devices));
      Services.Add(new EldatRx09Transceiver("COM3", bus));

      //Test turning a lamp on
      //Task.Delay(10000).ContinueWith((t) => lamp.TurnOnAsync());
    }

    public void Stop()
    {
      _log.Warn("Stopping...");
      _log.Debug("Stopping devices...");
      Devices.Stop();
      _log.Debug("Stopping services...");
      Services.Stop();
      var ser = new XmlSerializer(typeof(VirtualHouse));
      using var w = new StreamWriter(@"c:\temp\settings.xml");
      ser.Serialize(w, this);
    }

    public void Start()
    {
      _log.Warn("Starting...");
      _log.Debug("Starting services...");
      Services.Start();
      _log.Debug("Starting devices...");
      Devices.Start();
    }


  }
}
