using System.Collections.Generic;

namespace Domain
{
    public interface ICanStartAndStopList<T> : IEnumerable<T> where T : ICanStartAndStop
    {
        void Add(T device);
        void Remove(T device);
        void Start();
        void Stop();
    }
}