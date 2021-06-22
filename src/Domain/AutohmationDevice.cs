using System;

namespace Domain
{
    public abstract class AutohmationDevice : IDevice
    {
        private bool _isDisposed;
        private string _name;

        protected AutohmationDevice(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;
                _name = value;
                NameChanged?.Invoke(this, _name);
            }
        }

        public event EventHandler<string>? NameChanged;

        public void Dispose()
        {
            if (_isDisposed) return;
            Stop();
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }

        public abstract void Start();
        public abstract void Stop();
    }

}
