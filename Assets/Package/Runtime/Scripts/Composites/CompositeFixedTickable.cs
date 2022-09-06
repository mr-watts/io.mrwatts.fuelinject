using System.Collections.Generic;
using System.Linq;

namespace MrWatts.Internal.FuelInject
{
    public sealed class CompositeFixedTickable : IFixedTickable
    {
        private readonly IEnumerable<IFixedTickable> delegates;

        public CompositeFixedTickable(IOrderedEnumerable<IFixedTickable> delegates)
        {
            this.delegates = delegates;
        }

        public void FixedTick()
        {
            foreach (IFixedTickable delegatee in delegates)
            {
                delegatee.FixedTick();
            }
        }
    }
}