using System.Collections.Generic;
using System.Linq;

namespace MrWatts.Internal.FuelInject
{
    public sealed class CompositeInitializable : IInitializable
    {
        private readonly IEnumerable<IInitializable> delegates;

        public CompositeInitializable(IOrderedEnumerable<IInitializable> delegates)
        {
            this.delegates = delegates;
        }

        public void Initialize()
        {
            foreach (IInitializable delegatee in delegates)
            {
                delegatee.Initialize();
            }
        }
    }
}