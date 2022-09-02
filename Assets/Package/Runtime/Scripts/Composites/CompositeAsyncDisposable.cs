// using System;
// using System.Threading.Tasks;
// using System.Collections.Generic;

namespace MrWatts.Internal.FuelInject
{
    /*
        NOTE: Currently not conveniently possible to implement in Unity 2021 because IAsyncDisposable is part of .NET
        Standard 2.1 by default, but Unity will complain the Microsoft.Bcl.AsyncInterfaces shim from NuGetForUnity is
        necessary (incorrectly, because the shim also mentions it only applies to .NET Standard <= 2.0). If you add the
        package anyway, Unity will complain that there are duplicates.

        See also
            - https://forum.unity.com/threads/unity-future-net-development-status.1092205/page-6#post-7466252
            - https://forum.unity.com/threads/unity-future-net-development-status.1092205/page-10#post-7658806
            - https://issuetracker.unity3d.com/issues/dot-netstandard-2-dot-1-in-the-editor-is-missing-system-dot-memory-system-dot-buffers-at-runtime

        The issue was supposedly fixed in 2021.2, but the second post implies it isn't until 2022 (and it also still
        occurs at least in 2021.3.6).
    */
    // public sealed class CompositeAsyncDisposable : IAsyncDisposable
    // {
    //     private readonly IEnumerable<IAsyncDisposable> delegates;

    //     public CompositeAsyncDisposable(IEnumerable<IAsyncDisposable> delegates)
    //     {
    //         this.delegates = delegates;
    //     }

    //     public async ValueTask DisposeAsync()
    //     {
    //         foreach (IAsyncDisposable delegatee in delegates)
    //         {
    //             await delegatee.DisposeAsync();
    //         }
    //     }
    // }
}