using System;

namespace MrWatts.Internal.FuelInject.TestProject.Tests.Behaviour
{
    /// <summary>
    /// Module that, on Configure, executes the specified callback.
    /// </summary>
    internal sealed class CallbackInvokingTickable : ITickable
    {
        private readonly Action callback;

        public CallbackInvokingTickable(Action callback)
        {
            this.callback = callback;
        }

        public void Tick()
        {
            callback();
        }
    }
}