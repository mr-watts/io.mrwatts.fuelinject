using System;

namespace MrWatts.Internal.FuelInject.TestProject.Tests.Behaviour
{
    internal sealed class CallbackInvokingLateTickable : ILateTickable
    {
        private readonly Action callback;

        public CallbackInvokingLateTickable(Action callback)
        {
            this.callback = callback;
        }

        public void LateTick()
        {
            callback();
        }
    }
}