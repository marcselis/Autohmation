using log4net;
using MemBus;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Domain
{

    public class Lamp : AutohmationDevice, ISwitchableDevice
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Lamp));
        private readonly IBus _bus;
        private IDisposable _onSubscription;
        private IDisposable _offSubscription;

        public Lamp(string name, IServiceProvider services, string attatchedTo)
        {
            Name = name;
            _bus = services.GetService<IBus>();
            AttatchedTo = attatchedTo;
        }

        private void SetState(string name, State state)
        {
            if (AttatchedTo != name)
            {
                return;
            }

            State = state;
            Log.Info($"Lamp {Name} is switched {state}");
            StateChanged?.Invoke(this, State);
        }


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

        public override void Start()
        {
            Log.Debug($"Lamp {Name} is starting...");
            _onSubscription = _bus.Subscribe((SwitchedOn msg) => SetState(msg.Name, State.On));
            _offSubscription = _bus.Subscribe((SwitchedOff msg) => SetState(msg.Name, State.Off));
        }

        public override void Stop()
        {
            Log.Debug($"Lamp {Name} is stopping...");
            _offSubscription?.Dispose();
            _onSubscription?.Dispose();
        }
    }
}
