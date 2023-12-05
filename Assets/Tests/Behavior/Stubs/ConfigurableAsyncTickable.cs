using System;
using System.Threading.Tasks;

namespace MrWatts.Internal.FuelInject.TestProject.Tests.Behaviour
{
    internal sealed class ConfigurableAsyncTickable : IAsyncTickable
    {
        public readonly Task taskToWaitFor;

        public bool HasTicked { get; private set; }

        public EventHandler? OnTick;

        public ConfigurableAsyncTickable(Task taskToWaitFor)
        {
            this.taskToWaitFor = taskToWaitFor;
        }

        public async ValueTask TickAsync()
        {
            await taskToWaitFor;

            HasTicked = true;

            OnTick?.Invoke(this, EventArgs.Empty);
        }

        public void Reset()
        {
            HasTicked = false;
        }
    }
}