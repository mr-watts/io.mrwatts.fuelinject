using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace MrWatts.Internal.FuelInject
{
    public sealed class CompositeAsyncInitializable : IAsyncInitializable
    {
        private readonly IEnumerable<IAsyncInitializable> delegates;

        public CompositeAsyncInitializable(IOrderedEnumerable<IAsyncInitializable> delegates)
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