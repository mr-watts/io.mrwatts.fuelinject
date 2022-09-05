using System;
using Autofac;

namespace MrWatts.Internal.FuelInject.Testing
{
    public sealed class SceneSetupParameters
    {
        /// <summary>
        /// Name of the scene to load.
        /// </summary>
        public string SceneName { get; set; }

        /// <summary>
        /// Callback that is invoked after all existing module container bindings have been registered. This allows
        /// overriding application bindings for the purposes of testing.
        /// </summary>
        public Action<ContainerBuilder>? ContainerBindingCallback { get; set; }

        /// <summary>
        /// Whether or not to inject additional listeners into the container automatically that allow listening to
        /// kernel events (such as IInitializable's finishing).
        /// </summary>
        public bool AttachKernelListeners { get; set; } = true;

        public SceneSetupParameters(string sceneName)
        {
            SceneName = sceneName;
        }
    }
}