namespace Domain
{
    public interface ICanTurnOnAndOff
    {
        /// <summary>
        /// Gets the current state of the device.
        /// </summary>
        State State { get; }

     }
}
