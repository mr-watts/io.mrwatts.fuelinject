using System.Collections.Generic;

namespace MrWatts.Internal.FuelInject
{
    public sealed class CompositeLateTickable : ILateTickable
    {
        private readonly IEnumerable<ILateTickable> delegates;

        public CompositeLateTickable(IEnumerable<ILateTickable> delegates)
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