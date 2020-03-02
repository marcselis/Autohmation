namespace Domain
{
    public class Subscription : IEasywaveSubscription
    {

        public Subscription(uint address, KeyCode keyCode, bool isFromTransceiver = false)
        {
            Address = address;
            KeyCode = keyCode;
            IsFromTransceiver = isFromTransceiver;
        }

        public uint Address { get; }
        public KeyCode KeyCode { get; }
        public bool IsFromTransceiver { get; }
    }

}
