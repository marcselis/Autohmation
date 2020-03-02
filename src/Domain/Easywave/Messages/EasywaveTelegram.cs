namespace Domain
{
    public struct EasywaveTelegram
    {
        public EasywaveTelegram(uint address, KeyCode keyCode)
        {
            Address = address;
            KeyCode = keyCode;
        }
        
        public uint Address { get; }
        public KeyCode KeyCode { get; }
    }
}