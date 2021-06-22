using System.Collections.Generic;

namespace Domain
{
    public interface ICanStartAndStopList<T> : IEnumerable<T> where T : ICanStartAndStop
    {
        void Add(T item);
        void Remove(T item);
        void Start();
        void Stop();
    }
}