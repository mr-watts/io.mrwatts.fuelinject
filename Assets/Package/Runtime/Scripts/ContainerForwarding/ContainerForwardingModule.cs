using Autofac;
using UnityEngine;

namespace MrWatts.Internal.FuelInject
{
    /// <summary>
    /// Module that locates ContainerForwardingRegistrationSources in all open scenes and registers them as fallback
    /// source for the container it is registered in. This allows sharing services between scenes.
    /// </summary>
    public sealed class ContainerForwardingModule : MonoBehaviour, IUnityContainerModule
    {
        [SerializeField]
        [Tooltip("Source to ignore (not register) when forwarding. Usually set to the one in the scene this module is in to avoid registering this source to its own module. May be left unassigned if there is none.")]
        private ContainerForwardingRegistrationSource containerForwardingRegistrationSourceToIgnore = default!;

        public void Configure(ContainerBuilder builder)
        {
            foreach (ContainerForwardingRegistrationSource source in FindObjectsByType<ContainerForwardingRegistrationSource>(FindObjectsSortMode.None))
            {
                if (containerForwardingRegistrationSourceToIgnore != source)
                {
                    builder.RegisterSource(source);
                }
            }
        }
    }
}