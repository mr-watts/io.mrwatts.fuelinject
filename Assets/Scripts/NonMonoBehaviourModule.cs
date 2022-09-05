using Autofac;
using UnityEngine;

namespace MrWatts.Internal.FuelInject.TestProject
{
    public sealed class NonMonoBehaviourModule : IUnityContainerModule
    {
        public void Configure(ContainerBuilder builder)
        {
            Debug.Log("Non-MonoBehaviour test module configuring container");
        }
    }
}