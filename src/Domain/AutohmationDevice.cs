using System;

namespace Domain
{
    public abstract class AutohmationDevice : IDevice
    {
        private bool _isDisposed;
        private string name;

        public string Name
        {
            get { return name; }
            protected set
            {
                if (name == value) return;
                name = value;
                NameChanged?.Invoke(this, name);
            }
        }

        public event EventHandler<string> NameChanged;

        public void Dispose()
        {
            if (_isDisposed) return;
            Stop();
            _isDisposed = true;
        }

        public abstract void Start();
        public abstract void Stop();
    }

}
