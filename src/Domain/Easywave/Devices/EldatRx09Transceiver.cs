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
    public sealed class EldatRx09Transceiver : IEasywaveTransceiver
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(EldatRx09Transceiver));
        private readonly IBus _bus;
        private readonly string _portName;
        private bool _isOpen;
        private SerialPort _port;
        private string buffer = string.Empty;
        private bool _isDisposed = false;

        public EldatRx09Transceiver(string port, IBus bus)
        {
            if (string.IsNullOrWhiteSpace(port))
                throw new ArgumentNullException(nameof(port));
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
            _portName = port;
            bus.Subscribe(async (RequestTransmission req) => await TransmitAsync(req.Telegram).ConfigureAwait(false));
            Open();
        }

        public string Name { get { return "RX09"; } }
 
        /// <summary>
        /// Gets the vendor id of the transceiver.
        /// </summary>
        /// <remarks>
        /// Works only after the transceiver is opened
        /// </remarks>
        public string VendorId { get; private set; }
        /// <summary>
        /// Gets the device id of the transceiver.
        /// </summary>
        /// <remarks>
        /// Works only after the transceiver is opened
        /// </remarks>
        public string DeviceId { get; private set; }
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
        public async Task TransmitAsync(EasywaveTelegram message)
        {
            if (!_isOpen)
                throw new NotSupportedException("Open the transceiver before transmitting");
            if (message.Address >= AddressCount)
                throw new ArgumentOutOfRangeException(nameof(message.Address),
                    $"Cannot transmit to address {message.Address}.  The adapter only supports {AddressCount} addresses.");
            var text = $"TXP,{message.Address:x2},{message.KeyCode}\r";
            Log.Debug(">" + text);
            _port.Write(text);
            await _bus.PublishAsync(message).ConfigureAwait(false);
        }

        /// <summary>
        /// Opens the transceiver and makes it listen for broadcasted Easywave <see cref="EasywaveTelegram"/>s.
        /// </summary>
        /// <remarks>
        /// This method also initializes the AddressCount, VendorId, DeviceId & Version properties with the
        /// information returned from the device.
        /// </remarks>
        public void Open()
        {
            _port = new SerialPort(_portName, 57600, Parity.None, 8, StopBits.One)
            {
                Handshake = Handshake.None,
                DtrEnable = true,
                RtsEnable = true
            };
            _port.DataReceived += DataReceived;
            _port.ErrorReceived += ErrorReceived;
            _port.Open();
            //Ask for number of addresses and vendor/device id.
            //The responses will be processed by the DataReceived 
            //method and will be used to initialize the AddressCount, VendorId, DeviceId & Version properties.
            _port.Write("GETP?\r");
            _port.Write("ID?\r");
            _isOpen = true;
        }

        /// <summary>
        /// Closes the transceiver so that it stops listening.
        /// </summary>
        public void Close()
        {
            if (!_isOpen) return;
            _port.DataReceived -= DataReceived;
            _port.ErrorReceived -= ErrorReceived;
            _port.Close();
            _port.Dispose();
        }

        private static void ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Log.Debug($"Error received from serial port: {e.EventType}");
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType == SerialData.Chars) //no need to process if we received an EOF
            {
                var port = (SerialPort)sender;
                //Concat the new received characters to the unprocessed input of the previous read.
                //There is very little chance that there will ever be any unprocessed input, but just to be sure...
                buffer = string.Concat(buffer, port.ReadExisting());
                ReadOnlySpan<char> input = buffer.AsSpan();
                //Process the buffer line by line (each Easywave telegram ends with \r)
                var pos = input.IndexOf('\r');
                while (pos > 0)
                {
                    var line = input.Slice(0, pos + 1).ToString();
                    ProcessLine(line);
                    input = input.Slice(pos + 1);
                    pos = input.IndexOf('\r');
                }
                buffer = input.ToString();
            }
        }

        private async void ProcessLine(string line)
        {
            Log.Debug($"<{line}");
            if (string.IsNullOrWhiteSpace(line)) return;
            var parts = line.Split(',', '\t', '\r');
            if (parts.Length == 0) return;
            switch (parts[0])
            {
                case "ID":
                    VendorId = parts[1];
                    DeviceId = parts[2];
                    Version = uint.Parse(parts[3], NumberStyles.HexNumber);
                    Log.Debug($"Reveived ID {VendorId}:{DeviceId} Version {Version}");
                    break;
                case "GETP":
                    AddressCount = uint.Parse(parts[1], NumberStyles.HexNumber);
                    Log.Debug($"Transceiver has {AddressCount} addresses");
                    break;
                case "REC":
                    var address = uint.Parse(parts[1], NumberStyles.HexNumber);
                    var code = (KeyCode)Enum.Parse(typeof(KeyCode), parts[2]);
                    await _bus.PublishAsync(new EasywaveTelegram(address, code)).ConfigureAwait(false);
                    break;
                case "OK":
                    break;
                default:
                    Log.Debug($"Unexpected input: {line}");
                    break;
            }
        }


        /// <summary>
        /// Frees up unmanaged resources
        /// </summary>
        public void Dispose()
        {
            //the simple form of the Dispose pattern is sufficient here as 
            //the SerialPort we are using is a managed resource which
            //holds an unmanaged resource.
            if (_isDisposed) return;
            if (_isOpen) Close();
            _isDisposed = true;
        }



    }

}