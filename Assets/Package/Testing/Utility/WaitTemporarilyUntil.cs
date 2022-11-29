using System;
using UnityEngine;

namespace MrWatts.Internal.FuelInject.Testing.Utility
{
    /// <summary>
    /// <para>Custom yield instruction that extends WaitUntil to throw an exception if waiting takes too long.</para>
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
        private readonly TimeSpan timeToWait;
        private readonly DateTime endDateTime;

        public override bool keepWaiting
        {
            get
            {
                if (DateTime.Now >= endDateTime)
                {
                    throw new TimeoutException($"Timed out waiting {timeToWait.TotalSeconds} seconds for predicate to return true");
                }

                return !predicate();
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="predicate">Predicate returning <see langword="true"/> if waiting should stop or <see langword="false"/> if not.</param>
        /// <param name="timeToWait">The time to wait at most. 10 seconds by default if not set.</param>
        public WaitTemporarilyUntil(Func<bool> predicate, TimeSpan? timeToWait = null)
        {
            this.predicate = predicate;
            this.timeToWait = timeToWait is null ? TimeSpan.FromSeconds(10) : (TimeSpan)timeToWait;

            endDateTime = DateTime.Now.Add(this.timeToWait);
        }
    }
}