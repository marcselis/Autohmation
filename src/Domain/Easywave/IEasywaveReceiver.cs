using System.Collections.Generic;

namespace Domain
{
    public interface IEasywaveReceiver : IDevice
    {
        IEnumerable<IEasywaveSubscription> Subscriptions { get; }
        void Receive(EasywaveTelegram telegram);
    }
}
