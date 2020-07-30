namespace Domain
{
    public interface ISwitchableDevice : IOnOffDevice
    {
        ISwitch AttatchedTo { get; set; }
    }
}
