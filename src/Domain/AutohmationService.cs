using System;

namespace Domain
{
    public abstract class AutohmationService : IService
    {
        protected IServiceProvider _services;
        private bool _isDisposed;

        public AutohmationService(IServiceProvider services)
        {
            _services = services;
        }

        public virtual void Dispose()
        {
            if (_isDisposed) return;
            Stop();
            _isDisposed = true;
        }

        public abstract void Start();
        public abstract void Stop();
    }
  
}
