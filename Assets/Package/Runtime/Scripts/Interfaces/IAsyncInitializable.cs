using System.Threading.Tasks;

namespace MrWatts.Internal.DependencyInjection
{
    public interface IAsyncInitializable
    {
        ValueTask InitializeAsync();
    }
}