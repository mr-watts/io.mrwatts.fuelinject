using System;

namespace MrWatts.Internal.FuelInject.TestProject.Tests.Behaviour
{
    internal sealed class ListeningInitializable : IInitializable
    {
        public bool IsInitialized { get; private set; }

        public EventHandler? OnInitialized;

        public void Initialize()
        {
            IsInitialized = true;

            OnInitialized?.Invoke(this, EventArgs.Empty);
        }
    }
}