namespace Domain
{
    /// <summary>
    /// Interface for a device that can be turned on and off.
    /// </summary>
    public interface IOnOffDevice : IDevice, ICanTurnOnAndOff
    {
    }
}
