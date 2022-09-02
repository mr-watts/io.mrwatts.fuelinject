using System.Threading.Tasks;

namespace MrWatts.Internal.FuelInject
{
    public interface IAsyncInitializable
    {
        ValueTask InitializeAsync();
    }
}