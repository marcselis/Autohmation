using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// Interface for a device that can be turned on and off.
    /// </summary>
    public interface IOnOffDevice : IDevice
    {
        /// <summary>
        /// Gets the current state of the device.
        /// </summary>
        State State { get; }
        /// <summary>
        /// Turns the device onasynchronously.
        /// </summary>
        /// <returns>A <see cref="Task"/> instance that can be used to monitor the call.</returns>
        Task TurnOnAsync();
        /// <summary>
        /// Turns the device off asynchronously.
        /// </summary>
        /// <returns>A <see cref="Task"/> instance that can be used to monitor the call.</returns>
        Task TurnOffAsync();
    }
}
