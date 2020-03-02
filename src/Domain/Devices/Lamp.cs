using log4net;
using MemBus;
using System;
using System.Threading.Tasks;

namespace Domain
{

    public class Lamp : ISwitchableDevice
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Lamp));
        private readonly IBus _bus;
        private readonly IDisposable _onSubscription;
        private readonly IDisposable _offSubscription;
        private bool _isDisposed;

        public Lamp() { }
        public Lamp(string name, IBus bus, string attatchedTo)
        {
            Name = name;
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
            _onSubscription = _bus.Subscribe((SwitchedOn msg) => SetState(msg.Name, State.On));
            _offSubscription = _bus.Subscribe((SwitchedOff msg) => SetState(msg.Name, State.Off));
            AttatchedTo = attatchedTo;
        }

        private void SetState(string name, State state)
        {
            if (AttatchedTo != name)
            {
                return;
            }

            State = state;
            Log.Debug($"Lamp {Name} is switched {state}");
            StateChanged?.Invoke(this, State);
        }

        public string Name { get; }

        public string AttatchedTo { get; set; }

        public State State { get; private set; } = State.Off;

        public event EventHandler<State> StateChanged;


        public async Task TurnOnAsync()
        {
            if (State == State.On) return;
            Log.Debug($"Lamp {Name} is asked to turn on");
            if (string.IsNullOrEmpty(AttatchedTo))
            {
                throw new NotSupportedException("Lamp not attached to switch");
            }

            await _bus.PublishAsync(new RequestOn(AttatchedTo)).ConfigureAwait(false);
        }

        public async Task TurnOffAsync()
        {
            if (State == State.Off) return;
            Log.Debug($"Lamp {Name} is asked to turn off");
            if (string.IsNullOrEmpty(AttatchedTo))
            {
                throw new NotSupportedException("Lamp not attached to switch");
            }
            await _bus.PublishAsync(new RequestOff(AttatchedTo)).ConfigureAwait(false);
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _offSubscription.Dispose();
            _onSubscription.Dispose();
            _isDisposed = true;
        }
    }
}
