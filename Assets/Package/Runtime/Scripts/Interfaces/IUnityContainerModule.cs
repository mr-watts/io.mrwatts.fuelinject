using Autofac;

namespace MrWatts.Internal.DependencyInjection
{
    /// <summary>
    /// Exposes what Autofac's Module.Load does, but allows implementing an interface so you can still extend from
    /// MonoBehaviour for Unity.
    /// </summary>
    public interface IUnityContainerModule
    {
        /// <summary>
        /// Add registrations to the container.
        /// </summary>
        /// <remarks>
        /// Note that the ContainerBuilder parameter is unique to this module.
        /// </remarks>
        /// <param name="builder">The builder through which components can be registered.</param>
        void Configure(ContainerBuilder builder);
    }
}