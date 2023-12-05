using System.Threading.Tasks;

namespace MrWatts.Internal.FuelInject
{
    public interface IAsyncTickable
    {
        ValueTask TickAsync();
    }
}