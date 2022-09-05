using System.Threading.Tasks;

namespace MrWatts.Internal.FuelInject.TestProject.Tests.Behaviour
{
    internal sealed class ConfigurableAsyncInitializable : IAsyncInitializable
    {
        public readonly Task taskToWaitFor;

        public bool IsInitialized { get; private set; }

        public ConfigurableAsyncInitializable(Task taskToWaitFor)
        {
            this.taskToWaitFor = taskToWaitFor;
        }

        public async ValueTask InitializeAsync()
        {
            await taskToWaitFor;

            IsInitialized = true;
        }
    }
}