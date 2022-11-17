using System;

namespace MrWatts.Internal.FuelInject.TestProject.Tests.Behaviour
{
    internal sealed class CallbackInvokingFixedTickable : IFixedTickable
    {
        private readonly Action callback;

        public CallbackInvokingFixedTickable(Action callback)
        {
            this.callback = callback;
        }

        public void FixedTick()
        {
            callback();
        }
    }
}