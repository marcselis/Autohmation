namespace Domain
{
    public interface ISwitchableDevice : ICanBeTurnedOnAndOffDevice
    {
        string AttachedTo { get; set; }
    }
}
