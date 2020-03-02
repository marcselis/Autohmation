namespace Domain
{
    public abstract class AutohmationDevice : IDevice
    {
        private bool _isDisposed;

        public string Name { get; protected set; }

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
