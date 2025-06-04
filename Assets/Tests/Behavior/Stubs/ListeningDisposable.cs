using System;

namespace MrWatts.Internal.FuelInject.TestProject.Tests.Behaviour
{
    internal sealed class ListeningDisposable : IDisposable
    {
        public bool IsDisposed { get; private set; }

        public EventHandler? OnDisposed;

        public void Dispose()
        {
            IsDisposed = true;

            OnDisposed?.Invoke(this, EventArgs.Empty);
        }
    }
}