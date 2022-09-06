using System.Collections.Generic;
using System.Linq;

namespace MrWatts.Internal.FuelInject
{
    public sealed class CompositeLateTickable : ILateTickable
    {
        private readonly IEnumerable<ILateTickable> delegates;

        public CompositeLateTickable(IOrderedEnumerable<ILateTickable> delegates)
        {
            this.delegates = delegates;
        }

        public void LateTick()
        {
            foreach (ILateTickable delegatee in delegates)
            {
                delegatee.LateTick();
            }
        }
    }
}