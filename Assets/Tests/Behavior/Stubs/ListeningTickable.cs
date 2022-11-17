using System;

namespace MrWatts.Internal.FuelInject.TestProject.Tests.Behaviour
{
    internal sealed class ListeningTickable : ITickable
    {
        public bool HasTicked { get; private set; }

        public EventHandler? OnTick;

        public void Tick()
        {
            HasTicked = true;

            OnTick?.Invoke(this, EventArgs.Empty);
        }
    }
}