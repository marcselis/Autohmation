using System.Threading.Tasks;

namespace Domain
{
    public delegate void EventHandler<in TSender, in TArgs>(TSender sender, TArgs args);
    /// <summary>
    /// Abstraction of an Easywave Transceiver, a device that can send and receive Easywave Telegrams.
    /// A virtual house uses a transceiver for 2 purposes:
    /// 1) To detect what physical switches were pressed in the house and to update its state to the physical world.
    ///    For example, a lamp is connected to an easywave receiver that is paired with a certain button.
    ///    When that button is pressed the lamp will turn on.
    ///    Inside our virtual house, we would like to see that this lamp is turned on as well.
    /// 2) To send commands to Easywave receivers.  For example, to turn on the lamp without pressing the physical 
    ///    button. For this to work, the receiver needs to be paired to this command.
    /// By combining these 2 with other things, like time opens up interesting possibilities like "if the bedroom
    /// light is turned of after 1am, automatically turn off other lights that might be on in the house".
    /// </summary>
    public interface IEasywaveTransceiver : IDevice
    {
        uint AddressCount { get; }
        string DeviceId { get; }
        string VendorId { get; }
        uint Version { get; }

        void Close();
        void Open();
        Task TransmitAsync(EasywaveTelegram message);
    }
}