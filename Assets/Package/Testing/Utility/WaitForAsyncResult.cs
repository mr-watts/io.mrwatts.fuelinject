using System;
using UnityEngine;

namespace MrWatts.Internal.FuelInject.Testing.Utility
{
    /// <summary>
    /// <para>Custom yield instruction that allows waiting for IAsyncResult (Tasks, ValueTasks, ...) in Unity coroutines.</para>
    /// <para>
    /// This will only wait until the async result is completed (i.e. cancelled, failed, or successfully completed), and
    /// will not throw exceptions if the Task fails. The caller is responsible for checking the status afterwards.
    /// </para>
    /// </summary>
    /// <example>
    /// <code>
    /// public IEnumerator MyCoroutine()
    /// {
    ///     yield return new WaitForAsyncResult(Task.CompletedTask);
    ///     yield return new WaitForAsyncResult(fooTask);
    ///     yield return new WaitForAsyncResult(barValueTask);
    ///     yield return new WaitForAsyncResult(bazAsync());
    /// }
    /// </code>
    /// </example>
    public sealed class WaitForAsyncResult : CustomYieldInstruction
    {
        private readonly IAsyncResult result;

        public override bool keepWaiting => !result.IsCompleted;

        public WaitForAsyncResult(IAsyncResult result)
        {
            this.result = result;
        }
    }
}