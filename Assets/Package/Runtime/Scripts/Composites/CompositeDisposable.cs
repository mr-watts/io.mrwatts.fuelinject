using System;
using System.Collections.Generic;
using System.Linq;

namespace MrWatts.Internal.FuelInject
{
    public sealed class CompositeDisposable : IDisposable
    {
        private readonly IEnumerable<IDisposable> delegates;

        public CompositeDisposable(IOrderedEnumerable<IDisposable> delegates)
        {
            this.delegates = delegates;
        }

        public void Dispose()
        {
            foreach (IDisposable delegatee in delegates)
            {
                delegatee.Dispose();
            }
        }
    }
}