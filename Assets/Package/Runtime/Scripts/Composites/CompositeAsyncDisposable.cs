/*
    IAsyncDisposable is part of .NET Standard 2.1 by default, but Unity 2021 will complain the
    Microsoft.Bcl.AsyncInterfaces shim from NuGet is necessary (incorrectly, because the shim also mentions it only
    applies to .NET Standard <= 2.0). If you add the package anyway, Unity will then complain that there are duplicates.

    See also
        - https://forum.unity.com/threads/unity-future-net-development-status.1092205/page-6#post-7466252
        - https://forum.unity.com/threads/unity-future-net-development-status.1092205/page-10#post-7658806
        - https://issuetracker.unity3d.com/issues/dot-netstandard-2-dot-1-in-the-editor-is-missing-system-dot-memory-system-dot-buffers-at-runtime

    The issue was supposedly fixed in 2021.2, but still occurs in at least 2021.3.6. The second post implies it isn't
    fixed until 2022.
*/
#if UNITY_2022_1_OR_NEWER
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MrWatts.Internal.FuelInject
{
    public sealed class CompositeAsyncDisposable : IAsyncDisposable
    {
        private readonly IEnumerable<IAsyncDisposable> delegates;

        public CompositeAsyncDisposable(IOrderedEnumerable<IAsyncDisposable> delegates)
        {
            this.delegates = delegates;
        }

        public async ValueTask DisposeAsync()
        {
            foreach (IAsyncDisposable delegatee in delegates)
            {
                await delegatee.DisposeAsync();
            }
        }
    }
}
#endif