using System.Threading.Tasks;
using System.Collections.Generic;

namespace MrWatts.Internal.FuelInject
{
    public sealed class CompositeAsyncInitializable : IAsyncInitializable
    {
        private readonly IEnumerable<IAsyncInitializable> delegates;

        public CompositeAsyncInitializable(IEnumerable<IAsyncInitializable> delegates)
        {
            this.delegates = delegates;
        }

        public async ValueTask InitializeAsync()
        {
            foreach (IAsyncInitializable delegatee in delegates)
            {
                await delegatee.InitializeAsync();
            }
        }
    }
}