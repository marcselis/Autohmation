using log4net;
using MemBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain
{
    public class SimpleReceiver : ISimpleEasywaveReceiver
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SimpleReceiver));
        private readonly List<Subscription> _subscriptions = new List<Subscription>();
        private bool _isDisposed;
        private State _state;
        private readonly IDisposable _telegramSubscription;
        private readonly IDisposable _onSubsciption;
        private readonly IDisposable _offSubsciption;
        private readonly IBus _bus;


        public SimpleReceiver(string name, IBus bus, params Subscription[] subscription)
        {
            Name = name;
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
            _subscriptions.AddRange(subscription);
            _telegramSubscription = bus.Subscribe((EasywaveTelegram t) => Receive(t));
            _onSubsciption = bus.Subscribe(async (RequestOn msg) => await ProcessOnRequestAsync(msg).ConfigureAwait(false));
            _offSubsciption = bus.Subscribe(async (RequestOff msg) => await ProcessOffRequest(msg).ConfigureAwait(false));
        }

        public State State
        {
            get => _state;
            private set
            {
                if (_state == value)
                {
                    return;
                }

                _state = value;
                Log.Debug($"Receiver {Name} is switched {_state}");
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

        public string Name { get; }


        public IEnumerable<IEasywaveSubscription> Subscriptions => _subscriptions;


        public void Receive(EasywaveTelegram telegram)
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
            if (msg.SwitchName != Name) return;
            await TurnOffAsync().ConfigureAwait(false);
        }

        public async Task TurnOffAsync()
        { 
            Subscription sub = _subscriptions.FirstOrDefault(s => s.IsFromTransceiver);
            if (sub == null)
            {
                throw new NotSupportedException("Receiver has no triggerable subscription");
            }
            Log.Debug($"Receiver {Name} received request to turn off");

            await _bus.PublishAsync(new RequestTransmission { Telegram = new EasywaveTelegram(sub.Address, sub.KeyCode + 1) }).ConfigureAwait(false);
        }

        private async Task ProcessOnRequestAsync(RequestOn msg)
        {
            if (msg.SwitchName != Name) return;
            await TurnOnAsync().ConfigureAwait(false);
        }

        public async Task TurnOnAsync()
        { 
            Subscription sub = _subscriptions.FirstOrDefault(s => s.IsFromTransceiver);
            if (sub == null)
            {
                throw new NotSupportedException("Receiver has no triggerable subscription");
            }
            Log.Debug($"Receiver {Name} received request to turn on");
            await _bus.PublishAsync(new RequestTransmission { Telegram = new EasywaveTelegram(sub.Address, sub.KeyCode) }).ConfigureAwait(false);
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _telegramSubscription.Dispose();
            _onSubsciption.Dispose();
            _offSubsciption.Dispose();
            _isDisposed = true;
        }
    }
}
