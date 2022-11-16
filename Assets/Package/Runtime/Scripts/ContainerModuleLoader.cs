using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
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
        [Tooltip("Modules (implementing IUnityContainerModule) to load when the scene starts up. This is usually a module of your scene itself. If you consume other modules as well, such as from libraries, you usually want to register those using RegisterModule in the scene module itself instead of adding them here, so you can apply additional configuration if necessary.")]
        private Component[] startupModules = default!;

        private void Awake()
        {
            ContainerBuilder builder = new();

            LoadModules(builder);

            builder.RegisterSource(new OrderedEnumerableRegistrationSource());
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

            builder.RegisterType<ConfigurableComponentContextProxy>()
                .AsSelf()
                .As<IComponentContext>()
                .SingleInstance();

            IContainer container = builder.Build();

            container.Resolve<ConfigurableComponentContextProxy>().Delegatee = container;

            gameObject.AddComponent<UnityKernel>();

            container.Resolve<IInjector<Scene>>().Inject(gameObject.scene);
        }

        private void LoadModules(ContainerBuilder builder)
        {
            List<(int Priority, IModule Module)> modules = RetrieveStartupModules();

            if (automaticallyAddRootGameObjectModules)
            {
                modules.AddRange(RetrieveModulesForAutoload());
            }

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
                .Cast<(int Priority, IModule Module)>()
                .ToList();
        }

        private List<(int Priority, IModule Module)> RetrieveModulesForAutoload()
        {
            List<(int Priority, IModule Module)> modules = new();

            for (int i = 0; i < SceneManager.sceneCount; ++i)
            {
                modules.AddRange(RetrieveRootGameObjectModulesFromScene(SceneManager.GetSceneAt(i)));
            }

            return modules;
        }

        private List<(int Priority, IModule Module)> RetrieveModulesFromGameObject(GameObject gameObject)
        {
            return gameObject
                .GetComponents<Component>()
                .Select(TryRetrieveModuleFromComponent)
                .Where(x => x.Module is not null)
                .Cast<(int Priority, IModule Module)>()
                .ToList();
        }

        private (int Priority, IModule? Module) TryRetrieveModuleFromComponent(Component component)
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

            return (component is IPrioritizable prioritizable ? prioritizable.Priority : DEFAULT_MODULE_PRIORITY, module);
        }

        private List<(int Priority, IModule Module)> RetrieveRootGameObjectModulesFromScene(Scene scene)
        {
            return scene.GetRootGameObjects().SelectMany(RetrieveModulesFromGameObject).ToList();
        }
    }
}