using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace MrWatts.Internal.FuelInject
{
    public sealed class CompositeAsyncTickable : IAsyncTickable
    {
        private readonly IEnumerable<IAsyncTickable> delegates;

        public CompositeAsyncTickable(IOrderedEnumerable<IAsyncTickable> delegates)
        {
            this.delegates = delegates;
        }

        public async ValueTask TickAsync()
        {
            foreach (IAsyncTickable delegatee in delegates)
            {
                await delegatee.TickAsync();
            }
        }
    }
}