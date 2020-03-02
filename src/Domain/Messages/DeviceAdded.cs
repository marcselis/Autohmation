namespace Domain
{
    internal class DeviceAdded
    {
        public IDevice Device { get; }

        public DeviceAdded(IDevice device)
        {
            Device = device;
        }
    }
}