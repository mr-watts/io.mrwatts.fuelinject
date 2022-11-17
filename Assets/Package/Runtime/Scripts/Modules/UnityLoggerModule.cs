using Autofac;
using UnityEngine;

namespace MrWatts.Internal.FuelInject
{
    /// <summary>
    /// Module creating service bindings necessary for logging using Unity loggers.
    /// </summary>
    public sealed class UnityLoggerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(Debug.unityLogger).As<ILogger>().As<ILogHandler>();
            builder.RegisterType<UnityLoggerUnityKernelLoggerAdapter>().As<UnityLoggerUnityKernelLoggerAdapter>();
        }
    }
}