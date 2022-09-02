using Autofac;

namespace MrWatts.Internal.FuelInject
{
    /// <summary>
    /// Thin wrapper around Autofac's module to not force module developers to inherit from Module, as it prevents
    /// inheriting from MonoBehaviour. Autofac's Module also calls methods that are internal, so its code cannot be
    /// mimicked without inheriting.
    /// </summary>
    public sealed class UnityContainerAutofacModule : Module
    {
        private readonly IUnityContainerModule unityContainerModule;

        public UnityContainerAutofacModule(IUnityContainerModule unityContainerModule)
        {
            this.unityContainerModule = unityContainerModule;
        }

        protected override void Load(ContainerBuilder builder)
        {
            unityContainerModule.Configure(builder);
        }
    }
}