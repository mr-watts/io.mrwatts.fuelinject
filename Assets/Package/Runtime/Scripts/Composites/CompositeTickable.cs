using System.Collections.Generic;
using System.Linq;

namespace MrWatts.Internal.FuelInject
{
    public sealed class CompositeTickable : ITickable
    {
        private readonly IEnumerable<ITickable> delegates;

        public CompositeTickable(IOrderedEnumerable<ITickable> delegates)
        {
            this.delegates = delegates;
        }

        public void Tick()
        {
            foreach (ITickable delegatee in delegates)
            {
                delegatee.Tick();
            }
        }
    }
}