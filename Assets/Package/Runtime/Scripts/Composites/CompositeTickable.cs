using System.Collections.Generic;

namespace MrWatts.Internal.DependencyInjection
{
    public sealed class CompositeTickable : ITickable
    {
        private readonly IEnumerable<ITickable> delegates;

        public CompositeTickable(IEnumerable<ITickable> delegates)
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