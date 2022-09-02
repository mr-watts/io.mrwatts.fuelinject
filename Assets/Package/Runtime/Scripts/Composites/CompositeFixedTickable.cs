using System.Collections.Generic;

namespace MrWatts.Internal.FuelInject
{
    public sealed class CompositeFixedTickable : IFixedTickable
    {
        private readonly IEnumerable<IFixedTickable> delegates;

        public CompositeFixedTickable(IEnumerable<IFixedTickable> delegates)
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