using System;
using Autofac;
using Autofac.Core;
using Autofac.Features.ResolveAnything;
using UnityEngine;

namespace MrWatts.Internal.FuelInject
{
    /// <summary>
    /// <para>Loads configured container modules, builds the container, and injects the necessary dependencies into
    /// objects in the scene.</para>
    /// <para>MonoBehaviours requiring dependencies preferably shouldn't use them before Start is called, as the only
    /// order that is guaranteed is that Awake (used by this class) is called before Start. If you want to talk to
    /// dependencies in Awake, you must guarantee the Scipt Execution Order through your project settings.</para>
    /// </summary>
    public sealed class ContainerModuleLoader : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Modules (implementing IUnityContainerModule) to load when the scene starts up. This is usually a module of your scene itself. If you consume other modules as well, such as from libraries, you usually want to register those using RegisterModule in the scene module itself instead of adding them here, so you can apply additional configuration if necessary.")]
        private Component[] startupModules = default!;

        private IContainer? container;

        private async void Awake()
        {
            ContainerBuilder builder = new();

            foreach (Component startupModule in startupModules)
            {
                if (startupModule is IUnityContainerModule unityModule)
                {
                    builder.RegisterModule(new UnityContainerAutofacModule(unityModule));
                }
                else if (startupModule is IModule module)
                {
                    builder.RegisterModule(module);
                }
            }

            builder.RegisterComposite<CompositeInitializable, IInitializable>();
            builder.RegisterComposite<CompositeAsyncInitializable, IAsyncInitializable>();
            builder.RegisterComposite<CompositeTickable, ITickable>();
            builder.RegisterComposite<CompositeFixedTickable, IFixedTickable>();
            builder.RegisterComposite<CompositeLateTickable, ILateTickable>();
            builder.RegisterComposite<CompositeDisposable, IDisposable>();
            // builder.RegisterComposite<CompositeAsyncDisposable, IAsyncDisposable>();

            container = builder.Build();

            InjectSceneDependencies(container);

            container.Resolve<IInitializable>().Initialize();

            await container.Resolve<IAsyncInitializable>().InitializeAsync();
        }

        private void Update()
        {
            container?.Resolve<ITickable>().Tick();
        }

        private void FixedUpdate()
        {
            container?.Resolve<IFixedTickable>().FixedTick();
        }

        private void LateUpdate()
        {
            container?.Resolve<ILateTickable>().LateTick();
        }

        private /*async*/ void OnDestroy()
        {
            container?.Resolve<IDisposable>().Dispose();

            // await container.Resolve<IAsyncDisposable>().DisposeAsync();
        }

        private void InjectSceneDependencies(IContainer container)
        {
            // Create a temporary container so we don't pollute the application's container with our dependencies.
            ContainerBuilder temporaryContainerBuilder = new();
            temporaryContainerBuilder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            temporaryContainerBuilder.Build().Resolve<UnitySceneDependencyInjector>().Inject(container);
        }
    }
}