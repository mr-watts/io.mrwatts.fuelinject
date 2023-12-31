using System;

namespace MrWatts.Internal.FuelInject.Testing
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