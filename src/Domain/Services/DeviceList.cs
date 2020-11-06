using System.Collections;
using System.Collections.Generic;

namespace Domain.Services
{
    public class DeviceList<T> : ICanStartAndStopList<T> where T : IDevice
    {
        private readonly Dictionary<string, T> _list = new Dictionary<string, T>();

        public void Add(T item)
        {
            _list.Add(item.Name, item);
        }

        public void Remove(T item)
        {
            _list.Remove(item.Name);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Start()
        {
            foreach (T item in _list.Values)
                item.Start();
        }

        public void Stop()
        {
            foreach (T item in _list.Values)
                item.Stop();
        }

    }

}
