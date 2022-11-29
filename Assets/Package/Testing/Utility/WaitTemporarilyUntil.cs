using System;
using UnityEngine;

namespace MrWatts.Internal.FuelInject.Testing.Utility
{
    /// <summary>
    /// <para>Custom yield instruction that allows waiting until a predicate returns true in Unity coroutines.</para>
    /// <para>See Unity's WaitUntil if you want to wait without timeout.</para>
    /// </summary>
    /// <example>
    /// <code>
    /// public IEnumerator MyCoroutine()
    /// {
    ///     yield return new WaitUntilTimeout(() => somethingHappened, TimeSpan.FromSeconds(5));
    /// }
    /// </code>
    /// </example>
    public sealed class WaitTemporarilyUntil : CustomYieldInstruction
    {
        private readonly Func<bool> predicate;
        private readonly DateTime endDateTime;

        public override bool keepWaiting => DateTime.Now < endDateTime && !predicate();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="predicate">Predicate returning <see langword="true"/> if waiting should stop or <see langword="false"/> if not.</param>
        /// <param name="timeToWait">The time to wait at most. 10 seconds by default if not set.</param>
        public WaitTemporarilyUntil(Func<bool> predicate, TimeSpan? timeToWait = null)
        {
            this.predicate = predicate;

            endDateTime = DateTime.Now.Add(timeToWait is null ? TimeSpan.FromSeconds(10) : (TimeSpan)timeToWait);
        }
    }
}