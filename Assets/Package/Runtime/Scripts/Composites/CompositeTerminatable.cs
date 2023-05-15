using System.Collections.Generic;
using System.Linq;

namespace MrWatts.Internal.FuelInject
{
    public sealed class CompositeTerminatable : ITerminatable
    {
        private readonly IEnumerable<ITerminatable> delegates;

        public CompositeTerminatable(IOrderedEnumerable<ITerminatable> delegates)
        {
            this.delegates = delegates;
        }

        public void Terminate()
        {
            foreach (ITerminatable delegatee in delegates)
            {
                delegatee.Terminate();
            }
        }
    }
}