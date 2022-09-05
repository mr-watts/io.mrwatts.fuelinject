namespace MrWatts.Internal.FuelInject.TestProject.Tests.Behaviour
{
    internal sealed class ConfigurableInitializable : IInitializable
    {
        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            IsInitialized = true;
        }
    }
}