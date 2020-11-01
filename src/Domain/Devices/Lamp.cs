using log4net;
using MemBus;
using System;
using System.Threading.Tasks;

namespace Domain
{

    public class Lamp : AutohmationDevice, ISwitchableDevice
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Lamp));
        private readonly IBus _bus;
        private IDisposable _onSubscription;
        private IDisposable _offSubscription;

        public Lamp(string name, string attachedTo, IBus bus)
        {
            Name = name;
            _bus = bus;
            AttachedTo = attachedTo;
        }

        private void SetState(string name, State state)
        {
            if (AttachedTo != name)
            {
                return;
            }

            State = state;
            Log.Info($"Lamp {Name} is switched {state}");
            StateChanged?.Invoke(this, State);
        }


        public string AttachedTo { get; set; }

        public State State { get; private set; } = State.Off;

        public event EventHandler<State> StateChanged;


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

        public Task TurnOnAsync()
        {
            if (State == State.On) return Task.CompletedTask;
            return _bus.PublishAsync(new RequestOn(AttachedTo));
        }

        public Task TurnOffAsync()
        {
            if (State == State.Off) return Task.CompletedTask; ;
            return _bus.PublishAsync(new RequestOff(AttachedTo));
        }
    }
}
