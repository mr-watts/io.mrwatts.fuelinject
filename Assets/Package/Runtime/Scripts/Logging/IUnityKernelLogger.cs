using System;

namespace MrWatts.Internal.FuelInject
{
    /// <summary>
    /// Interface for loggers that handle logging for the UnityKernel.
    /// </summary>
    public interface IUnityKernelLogger
    {
        void LogException(Exception exception);
    }
}