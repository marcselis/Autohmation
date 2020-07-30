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

        public Lamp(string name, IServiceProvider services, ISwitch attatchedTo)
        {
            Name = name;
            _bus = services.GetService<IBus>();
            AttatchedTo = attatchedTo;
        }

        private void SetState(string name, State state)
        {
            if (AttatchedTo?.Name != name)
            {
                return;
            }

            State = state;
            Log.Info($"Lamp {Name} is switched {state}");
            StateChanged?.Invoke(this, State);
        }


        public ISwitch AttatchedTo { get; set; }

        public State State { get; private set; } = State.Off;

        public event EventHandler<State> StateChanged;


        public Task TurnOnAsync()
        {
            if (State == State.On) return Task.CompletedTask;
            Log.Debug($"Lamp {Name} is asked to turn on");
            if (AttatchedTo==null)
            {
                throw new NotSupportedException("Lamp not attached to switch");
            }
            return AttatchedTo.TurnOnAsync();
        }

        public Task TurnOffAsync()
        {
            if (State == State.Off) return Task.CompletedTask;
            Log.Debug($"Lamp {Name} is asked to turn off");
            if (AttatchedTo == null)
            {
                throw new NotSupportedException("Lamp not attached to switch");
            }
            return AttatchedTo.TurnOffAsync();
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
