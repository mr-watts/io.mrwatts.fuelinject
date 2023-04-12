using Autofac;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MrWatts.Internal.FuelInject
{
    /// <summary>
    /// Module creating service bindings necessary for injection into Unity objects.
    /// </summary>
    public sealed class UnityInjectionModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
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

            builder.RegisterType<GameObjectInstantiator>()
                .AsSelf()
                .As<IGameObjectInstantiator>()
                .As<IInstantiator<GameObject>>()
                .SingleInstance();
        }
    }
}