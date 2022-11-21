using System;
using System.Threading.Tasks;

namespace MrWatts.Internal.FuelInject.Testing
{
    internal sealed class CallbackInvokingAsyncInitializable : IAsyncInitializable
    {
        private readonly Action callback;

        public CallbackInvokingAsyncInitializable(Action callback)
        {
            this.callback = callback;
        }

        public ValueTask InitializeAsync()
        {
            callback();

            return new ValueTask(Task.CompletedTask);
        }
    }
}