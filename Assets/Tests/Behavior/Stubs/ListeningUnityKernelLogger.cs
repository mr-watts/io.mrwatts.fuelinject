using System;

namespace MrWatts.Internal.FuelInject.TestProject.Tests.Behaviour
{
    internal sealed class ListeningUnityKernelLogger : IUnityKernelLogger
    {
        public bool WasExceptionLogged { get; private set; }

        public EventHandler? OnLogged;

        public void LogException(Exception exception)
        {
            WasExceptionLogged = true;

            OnLogged?.Invoke(this, EventArgs.Empty);
        }
    }
}