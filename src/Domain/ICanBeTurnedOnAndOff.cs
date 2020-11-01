using System.Threading.Tasks;

namespace Domain
{
    public interface ICanBeTurnedOnAndOff
    {
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
