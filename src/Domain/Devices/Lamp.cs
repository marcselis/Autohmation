using log4net;
using MemBus;
using System;
using System.Threading.Tasks;

namespace Domain
{

    public class Lamp : AutohmationDevice, ISwitchableDevice
    {
        private readonly static ILog _log = LogManager.GetLogger(typeof(Lamp));
        private readonly IBus _bus;
        private IDisposable? _onSubscription;
        private IDisposable? _offSubscription;

        public Lamp(string name, string attachedTo, IBus bus) : base(name)
        {
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
            _log.Info($"Lamp {Name} is switched {state}");
            StateChanged?.Invoke(this, State);
        }


        public string AttachedTo { get; set; }

        public State State { get; private set; } = State.Off;

        public event EventHandler<State>? StateChanged;


        public override void Start()
        {
            _log.Debug($"Lamp {Name} is starting...");
            _onSubscription = _bus.Subscribe((SwitchedOn msg) => SetState(msg.Name, State.On));
            _offSubscription = _bus.Subscribe((SwitchedOff msg) => SetState(msg.Name, State.Off));
        }

        public override void Stop()
        {
            _log.Debug($"Lamp {Name} is stopping...");
            _offSubscription?.Dispose();
            _onSubscription?.Dispose();
        }

        public Task TurnOnAsync()
        {
            if (State == State.On)
            {
                return Task.CompletedTask;
            }

            return _bus.PublishAsync(new RequestOn(AttachedTo));
        }

        public Task TurnOffAsync()
        {
            if (State == State.Off)
            {
                return Task.CompletedTask;
            }

            ;
            return _bus.PublishAsync(new RequestOff(AttachedTo));
        }
    }
}
