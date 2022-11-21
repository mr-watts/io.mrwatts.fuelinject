using System;

namespace MrWatts.Internal.FuelInject.TestProject.Tests.Behaviour
{
    internal sealed class CallbackInvokingInitializable : IInitializable
    {
        private readonly Action callback;

        public CallbackInvokingInitializable(Action callback)
        {
            this.callback = callback;
        }

        public void Initialize()
        {
            callback();
        }
    }
}