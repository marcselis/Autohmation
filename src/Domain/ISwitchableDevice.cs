namespace Domain
{
    public interface ISwitchableDevice : IOnOffDevice
    {
        string AttatchedTo { get; set; }
    }
}
