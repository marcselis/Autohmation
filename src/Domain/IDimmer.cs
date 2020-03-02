namespace Domain
{
    public interface IDimmer : IDevice
    {
        uint Level { get; }
        void Increase();
        void Decrease();
    }
}
