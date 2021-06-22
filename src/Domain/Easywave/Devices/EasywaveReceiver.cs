using MemBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;

namespace Domain
{
    public class EasywaveReceiver : AutohmationDevice, ISwitch
    {
        private readonly static ILog _log = LogManager.GetLogger(typeof(EasywaveReceiver));
        private readonly List<Subscription> _subscriptions = new List<Subscription>();
        private State _state;
        private IDisposable? _telegramSubscription;
        private IDisposable? _onSubsciption;
        private IDisposable? _offSubsciption;
        private readonly IBus _bus;


        public EasywaveReceiver(string name, IBus bus, params Subscription[] subscription) : base(name)
        {
            _bus = bus;
            _subscriptions.AddRange(subscription);
        }

        public State State
        {
            get { return _state; }
            private set
            {
                if (_state == value)
                {
                    return;
                }

                _state = value;
                _log.Debug($"Receiver {Name} is switched {_state}");
                if (_state == State.On)
                {
                    _bus.Publish(new SwitchedOn(Name));
                }
                else
                {
                    _bus.Publish(new SwitchedOff(Name));
                }
            }
        }

        private void Receive(EasywaveTelegram telegram)
        {
            if (_subscriptions.Any(s => s.Address == telegram.Address && s.KeyCode == telegram.KeyCode))
            {
                State = State.On;
            }

            if (_subscriptions.Any(s => s.Address == telegram.Address && s.KeyCode + 1 == telegram.KeyCode))
            {
                State = State.Off;
            }
        }

        private async Task ProcessOffRequest(RequestOff msg)
        {
            if (msg.SwitchName != Name)
            {
                return;
            }

            await TurnOffAsync().ConfigureAwait(false);
        }

        public Task TurnOffAsync()
        { 
            var sub = _subscriptions.FirstOrDefault(s => s.IsFromTransceiver);
            if (sub == null)
            {
                throw new NotSupportedException("Receiver has no triggerable subscription");
            }
            _log.Debug($"Receiver {Name} received request to turn off");

            return _bus.PublishAsync(new RequestTransmission { Telegram = new EasywaveTelegram(sub.Address, sub.KeyCode + 1) });
        }

        private async Task ProcessOnRequestAsync(RequestOn msg)
        {
            if (msg.SwitchName != Name)
            {
                return;
            }

            await TurnOnAsync().ConfigureAwait(false);
        }

        public Task TurnOnAsync()
        { 
            var sub = _subscriptions.FirstOrDefault(s => s.IsFromTransceiver);
            if (sub == null)
            {
                throw new NotSupportedException("Receiver has no triggerable subscription");
            }
            _log.Debug($"Receiver {Name} received request to turn on");
            return _bus.PublishAsync(new RequestTransmission { Telegram = new EasywaveTelegram(sub.Address, sub.KeyCode) });
        }

        public override void Start()
        {
            _log.Debug($"{nameof(EasywaveReceiver)} {Name} is starting...");
            _telegramSubscription = _bus.Subscribe((EasywaveTelegram t) => Receive(t));
            _onSubsciption = _bus.Subscribe(async (RequestOn msg) => await ProcessOnRequestAsync(msg).ConfigureAwait(false));
            _offSubsciption = _bus.Subscribe(async (RequestOff msg) => await ProcessOffRequest(msg).ConfigureAwait(false));
        }

        public override void Stop()
        {
            _log.Debug($"{nameof(EasywaveReceiver)} {Name} is stopping...");
            _telegramSubscription?.Dispose();
            _onSubsciption?.Dispose();
            _offSubsciption?.Dispose();
        }
    }
}
