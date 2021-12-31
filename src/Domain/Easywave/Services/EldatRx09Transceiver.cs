using log4net;
using MemBus;
using System;
using System.Globalization;
using System.IO.Ports;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// <see cref="IEasywaveTransceiver"/> implementation for the Eldat RX09 USB Easywave Transceiver.
    /// </summary>
    /// <remarks>
    /// You need to have the Eldat driver installed in order for this to work.
    /// <see cref="https://www.eldat.de/produkte/_div/rx09e_USBTcEasywaveInstall_XP_Win7.zip"/>
    /// </remarks>
    public sealed class EldatRx09Transceiver : AutohmationService
    {
        private readonly static ILog _log = LogManager.GetLogger(typeof(EldatRx09Transceiver));
        private readonly IBus _bus;
        private bool _isOpen;
        private readonly SerialPort _port;
        private string _buffer = string.Empty;
        private IDisposable? _subscription;

        public EldatRx09Transceiver(string port, IBus bus)
        {
            _bus = bus;
            _port = new SerialPort(port, 57600, Parity.None, 8, StopBits.One)
            {
                Handshake = Handshake.None,
                DtrEnable = true,
                RtsEnable = true,
                NewLine = "\r"
            };
            _port.DataReceived += DataReceivedAsync;
            _port.ErrorReceived += ErrorReceived;
        }

        public override void Start()
        {
            _log.Debug("EldatRx09Transceiver is starting...");
            _subscription = _bus.Subscribe(async (RequestTransmission req) => await TransmitAsync(req.Telegram).ConfigureAwait(false));
            Open();
        }

        public override void Stop()
        {
            _log.Debug("EldatRx09Transceiver is stopping...");
            _subscription?.Dispose();
            _subscription = null;
            Close();
        }


        /// <summary>
        /// Gets the vendor id of the transceiver.
        /// </summary>
        /// <remarks>
        /// Works only after the transceiver is opened
        /// </remarks>
        public string? VendorId { get; private set; }
        /// <summary>
        /// Gets the device id of the transceiver.
        /// </summary>
        /// <remarks>
        /// Works only after the transceiver is opened
        /// </remarks>
        public string? DeviceId { get; private set; }
        /// <summary>
        /// Returns the version number of the transceiver.
        /// </summary>
        /// <remarks>
        /// Works only after the transceiver is opened
        /// </remarks>
        public uint Version { get; private set; }
        /// <summary>
        /// Returns the number of addresses this transceiver supports
        /// </summary>
        /// <remarks>
        /// Works only after the transceiver is opened
        /// </remarks>
        public uint AddressCount { get; private set; }

        /// <summary>
        /// Transmits an Easywave telegram trough the transceiver
        /// </summary>
        /// <param name="message">An Easywave <see cref="EasywaveTelegram"/> with the payload to transmit.</param>
        /// <remarks>
        /// Any message that is transmitted is also raised as <see cref="TelegramReceived"/> to allow other interested 
        /// parties to be notified.
        /// </remarks>
        public Task TransmitAsync(EasywaveTelegram message)
        {
            if (!_isOpen)
            {
                throw new InvalidOperationException("Open the transceiver before transmitting");
            }

            if (message.Address >= AddressCount)
            {
                throw new InvalidOperationException($"Cannot transmit to address {message.Address}.  The adapter only supports {AddressCount} addresses.");
            }

            var text = $"TXP,{message.Address:x2},{message.KeyCode}";
            _log.Debug(">" + text);
            _port.WriteLine(text);
            return _bus.PublishAsync(message);
        }

        /// <summary>
        /// Opens the transceiver and makes it listen for broadcasted Easywave <see cref="EasywaveTelegram"/>s.
        /// </summary>
        /// <remarks>
        /// This method also initializes the AddressCount, VendorId, DeviceId & Version properties with the
        /// information returned from the device.
        /// </remarks>
        private void Open()
        {
            _port.Open();
            //Ask for number of addresses and vendor/device id.
            //The responses will be processed by the DataReceived 
            //method and will be used to initialize the AddressCount, VendorId, DeviceId & Version properties.
            _port.WriteLine("GETP?\rID?");
            _isOpen = true;
        }

        /// <summary>
        /// Closes the transceiver so that it stops listening.
        /// </summary>
        private void Close()
        {
            if (!_isOpen)
            {
                return;
            }

            _port.DataReceived -= DataReceivedAsync;
            _port.ErrorReceived -= ErrorReceived;
            _port.Close();
            _port.Dispose();
        }

        private static void ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            _log.Debug($"Error received from serial port: {e.EventType}");
        }

        private async void DataReceivedAsync(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType != SerialData.Chars)
            {
                return;
            }
            
            var port = (SerialPort)sender;
            var  line = port.ReadLine();
            while(line.Length>0)
            { 
                await ProcessLine(line).ConfigureAwait(false);
                line = port.ReadLine();
            }
        }

        private async Task ProcessLine(string line)
        {
            _log.Debug($"<{line}");
            if (string.IsNullOrWhiteSpace(line))
            {
                return;
            }

            var parts = line.Split(',', '\t');
            if (parts.Length == 0)
            {
                return;
            }

            switch (parts[0])
            {
                case "ID":
                    VendorId = parts[1];
                    DeviceId = parts[2];
                    Version = uint.Parse(parts[3], NumberStyles.HexNumber);
                    _log.Debug($"Reveived ID {VendorId}:{DeviceId} Version {Version}");
                    break;
                case "GETP":
                    AddressCount = uint.Parse(parts[1], NumberStyles.HexNumber);
                    _log.Debug($"Transceiver has {AddressCount} addresses");
                    break;
                case "REC":
                    var address = uint.Parse(parts[1], NumberStyles.HexNumber);
                    var code = (KeyCode)Enum.Parse(typeof(KeyCode), parts[2]);
                    await _bus.PublishAsync(new EasywaveTelegram(address, code)).ConfigureAwait(false);
                    break;
                case "OK":
                    break;
                default:
                    _log.Debug($"Unexpected input: {line}");
                    break;
            }
        }

    }

}