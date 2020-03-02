using System;

namespace Domain
{
    /// <summary>
    /// Generic interface for a device in our system.
    /// </summary>
    public interface IDevice : ICanStartAndStop,IDisposable
    {
        /// <summary>
        /// Gets or sets the device name.
        /// </summary>
        string Name { get; }

        //event EventHandler<string> NameChanged;
    }

}
