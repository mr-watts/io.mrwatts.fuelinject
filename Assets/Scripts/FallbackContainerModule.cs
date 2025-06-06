using Autofac;
using UnityEngine;

namespace MrWatts.Internal.FuelInject.TestProject
{
    internal sealed class FallbackContainerModule : MonoBehaviour, IUnityContainerModule
    {
        public void Configure(ContainerBuilder builder)
        {
            builder.RegisterType<FallbackService>().SingleInstance();
        }
    }
}