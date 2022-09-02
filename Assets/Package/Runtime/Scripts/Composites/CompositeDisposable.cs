using System;
using System.Collections.Generic;

namespace MrWatts.Internal.DependencyInjection
{
    public sealed class CompositeDisposable : IDisposable
    {
        private readonly IEnumerable<IDisposable> delegates;

        public CompositeDisposable(IEnumerable<IDisposable> delegates)
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