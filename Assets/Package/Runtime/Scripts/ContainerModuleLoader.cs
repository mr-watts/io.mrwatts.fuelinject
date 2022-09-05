using System;
using Autofac;
using Autofac.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        private void Awake()
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

            builder.RegisterComposite<CompositeInitializable, IInitializable>().SingleInstance();
            builder.RegisterComposite<CompositeAsyncInitializable, IAsyncInitializable>().SingleInstance();
            builder.RegisterComposite<CompositeTickable, ITickable>().SingleInstance();
            builder.RegisterComposite<CompositeFixedTickable, IFixedTickable>().SingleInstance();
            builder.RegisterComposite<CompositeLateTickable, ILateTickable>().SingleInstance();
            builder.RegisterComposite<CompositeDisposable, IDisposable>().SingleInstance();
            // builder.RegisterComposite<CompositeAsyncDisposable, IAsyncDisposable>();

            builder.RegisterType<ObjectDependencyInjector>()
                .AsSelf()
                .As<IInjector<object>>()
                .SingleInstance();

            builder.RegisterType<UnityGameObjectDependencyInjector>()
                .AsSelf()
                .As<IInjector<GameObject>>()
                .SingleInstance();

            builder.RegisterType<UnitySceneDependencyInjector>()
                .AsSelf()
                .As<IInjector<Scene>>()
                .SingleInstance();

            IContainer container = builder.Build();

            gameObject.AddComponent<UnityKernel>();

            container.Resolve<IInjector<Scene>>().Inject(SceneManager.GetActiveScene(), container);
        }
    }
}