using System;

namespace MrWatts.Internal.FuelInject.Testing
{
    /// <summary>
    /// Module that, on Configure, executes the specified callback.
    /// </summary>
    public sealed class CallbackInvokingInitializable : IInitializable
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