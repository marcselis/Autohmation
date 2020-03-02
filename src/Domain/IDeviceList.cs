using System.Collections.Generic;

namespace Domain
{
    public interface IDeviceList: IEnumerable<IDevice>
    {
        void Add(IDevice device);
        void Remove(IDevice device);
    }
}