using System.Collections.Generic;

namespace MrWatts.Internal.DependencyInjection
{
    public sealed class CompositeInitializable : IInitializable
    {
        private readonly IEnumerable<IInitializable> delegates;

        public CompositeInitializable(IEnumerable<IInitializable> delegates)
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