using System.Threading.Tasks;

namespace Domain
{
    public interface ISwitch
    {
        State State { get; }
        Task TurnOnAsync();
        Task TurnOffAsync();
    }
}
