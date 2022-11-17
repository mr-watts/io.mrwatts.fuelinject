using System;

namespace MrWatts.Internal.FuelInject.TestProject.Tests.Behaviour
{
    internal sealed class CallbackInvokingDisposable : IDisposable
    {
        private readonly Action callback;

        public CallbackInvokingDisposable(Action callback)
        {
            this.callback = callback;
        }

        public void Dispose()
        {
            callback();
        }
    }
}