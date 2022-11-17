using System;
using UnityEngine;

namespace MrWatts.Internal.FuelInject
{
    /// <summary>
    /// Adapter that adpts an Unity ILogger to the IUnityKernelLogger interface.
    /// </summary>
    public sealed class UnityLoggerUnityKernelLoggerAdapter : IUnityKernelLogger
    {
        private readonly ILogger logger;

        public UnityLoggerUnityKernelLoggerAdapter(ILogger logger)
        {
            this.logger = logger;
        }

        public void LogException(Exception exception)
        {
            logger.LogException(exception);
        }
    }
}