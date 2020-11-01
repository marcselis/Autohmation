using System;

namespace Domain
{
    public abstract class AutohmationService : IService
    {
        private bool _isDisposed;

  
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
