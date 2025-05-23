using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using Autofac.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MrWatts.Internal.FuelInject
{
    /// <summary>
    /// <para>Loads configured container modules, builds the container, and injects the necessary dependencies into
    /// objects in the scene.</para>
    /// <para>Dependencies will only be injected into objects that are in the scene this loader('s game object) is in.</para>
    /// <para>MonoBehaviours requiring dependencies preferably shouldn't use them before Start is called, as the only
    /// order that is guaranteed is that Awake (used by this class) is called before Start. If you want to talk to
    /// dependencies in Awake, you must guarantee the Scipt Execution Order through your project settings.</para>
    /// </summary>
    public sealed class ContainerModuleLoader : MonoBehaviour
    {
        private const int DEFAULT_MODULE_PRIORITY = 0;

        [SerializeField]
        [Tooltip("Whether to automatically scan root game objects in (all) scene(s) for modules and add them automatically. These then don't need to be added to start-up modules. This is especially useful if you use additive scenes, wish to dynamically load the scene with a loader, automatically letting modules from all scenes be inserted.")]
        private bool automaticallyAddRootGameObjectModules = true;

        [SerializeField]
        [Tooltip("Whether to automatically load the Unity logger (sub)module. You can disable this if you don't want to load UnityLoggerModule, or want to load it yourself to perform additional customizations to it.\n\nFuel Inject needs an IUnityKernelLogger to function correctly, so you are responsible for binding one yourself if you choose not to use the built-in UnityLoggerModule!")]
        private bool automaticallyLoadUnityLoggerModule = true;

        [SerializeField]
        [Tooltip("Whether to enable container diagnostics. This will automatically load the ContainerDiagnosticsModule for the current container, logging its output to the Unity console. If you want to log somewhere else, disable this and load the module yourself with a custom tracer.")]
        private bool enableContainerDiagnostics = false;

        [SerializeField]
        [Tooltip("Modules (implementing IUnityContainerModule) to load when the scene starts up. This is usually a module of your scene itself. If you consume other modules as well, such as from libraries, you usually want to register those using RegisterModule in the scene module itself instead of adding them here, so you can apply additional configuration if necessary.")]
        private Component[] startupModules = default!;

        private void Awake()
        {
            ContainerBuilder builder = new();

            LoadModules(builder);

            builder.RegisterSource(new OrderedEnumerableRegistrationSource());

            builder.RegisterType<ConfigurableComponentContextProxy>()
                .AsSelf()
                .As<IComponentContext>()
                .SingleInstance();

            IContainer container = builder.Build();

            /*
                Proxy the container (IComponentContext) through a proxy class, because the IComponentContext may not be
                bound directly using container.Register<IComponentContext>(context => context) as `context` may not be
                stored.
            */
            container.Resolve<ConfigurableComponentContextProxy>().Delegatee = container;

            gameObject.AddComponent<UnityKernel>();

            container.Resolve<IInjector<Scene>>().Inject(gameObject.scene);
        }

        private void LoadModules(ContainerBuilder builder)
        {
            if (enableContainerDiagnostics)
            {
                DefaultDiagnosticTracer tracer = new();
                tracer.OperationCompleted += (_, args) => Debug.unityLogger.Log("Container Diagnostics", args.TraceContent);

                builder.RegisterModule(new ContainerDiagnosticsModule(tracer));
            }

            List<(int Priority, IModule Module)> modules = RetrieveStartupModules();

            builder.RegisterModule(new KernelModule());
            builder.RegisterModule(new UnityInjectionModule());

            if (automaticallyLoadUnityLoggerModule)
            {
                builder.RegisterModule(new UnityLoggerModule());
            }

            modules.AddRange(RetrieveModulesToAutoload(automaticallyAddRootGameObjectModules));

            foreach (IModule module in modules.OrderBy(x => x.Priority).Select(x => x.Module))
            {
                builder.RegisterModule(module);
            }
        }

        private List<(int Priority, IModule Module)> RetrieveStartupModules()
        {
            return startupModules
                .Select(TryRetrieveModuleFromComponent)
                .Where(x => x.Module is not null)
                .Cast<(int Priority, bool AlwaysLoads, IModule Module)>()
                .Select(x => (x.Priority, x.Module))
                .ToList();
        }

        private List<(int Priority, IModule Module)> RetrieveModulesToAutoload(bool automaticallyAddRootGameObjectModules)
        {
            List<(int Priority, IModule Module)> modules = new();

            for (int i = 0; i < SceneManager.sceneCount; ++i)
            {
                List<(int Priority, bool AlwaysLoads, IModule Module)> modulesInScene = RetrieveRootGameObjectModulesFromScene(SceneManager.GetSceneAt(i));

                foreach ((int Priority, bool AlwaysLoads, IModule Module) moduleInScene in modulesInScene)
                {
                    if (automaticallyAddRootGameObjectModules || moduleInScene.AlwaysLoads)
                    {
                        modules.Add((moduleInScene.Priority, moduleInScene.Module));
                    }
                }

            }

            return modules;
        }

        private List<(int Priority, bool AlwaysLoads, IModule Module)> RetrieveModulesFromGameObject(GameObject gameObject)
        {
            return gameObject
                .GetComponents<Component>()
                .Select(TryRetrieveModuleFromComponent)
                .Where(x => x.Module is not null)
                .Cast<(int Priority, bool AlwaysLoads, IModule Module)>()
                .ToList();
        }

        private (int Priority, bool AlwaysLoads, IModule? Module) TryRetrieveModuleFromComponent(Component component)
        {
            IModule? module = null;

            if (component is IUnityContainerModule unityModule)
            {
                module = new UnityContainerAutofacModule(unityModule);
            }
            else if (component is IModule autofacModule)
            {
                module = autofacModule;
            }

            return (
                component is IPrioritizable prioritizable ? prioritizable.Priority : DEFAULT_MODULE_PRIORITY,
                component is IGlobalOverridingModule,
                module
            );
        }

        private List<(int Priority, bool AlwaysLoads, IModule Module)> RetrieveRootGameObjectModulesFromScene(Scene scene)
        {
            return scene.GetRootGameObjects().SelectMany(RetrieveModulesFromGameObject).ToList();
        }
    }
}