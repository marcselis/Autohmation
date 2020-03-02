using System.Collections;
using System.Collections.Generic;

namespace Domain.Services
{
    public class AutohmationList<T> : ICanStartAndStopList<T> where T : ICanStartAndStop
    {
        private readonly List<T> _list = new List<T>();
        public void Add(T item)
        {
            _list.Add(item);
        }

        public void Remove(T item)
        {
            _list.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Start()
        {
            foreach (T item in _list)
                item.Start();
        }

        public void Stop()
        {
            foreach (T item in _list)
                item.Stop();
        }
    }

}
