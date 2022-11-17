using System;
using Autofac;
using UnityEngine;

namespace MrWatts.Internal.FuelInject.Testing
{
    /// <summary>
    /// Module that, on Configure, executes the specified callback.
    /// </summary>
    internal sealed class CallbackInvokingModule : MonoBehaviour, IUnityContainerModule, IPrioritizable
    {
        public Action<ContainerBuilder>? Callback { get; set; }
        public int Priority { get; set; }

        public void Configure(ContainerBuilder builder)
        {
            if (Callback is null)
            {
                throw new Exception("Callback was still null by the time callback was invoked");
            }

            Callback(builder);
        }
    }
}