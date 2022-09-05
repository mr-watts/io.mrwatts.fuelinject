using Autofac;
using UnityEngine;

namespace MrWatts.Internal.FuelInject.TestProject
{
    public sealed class TestSceneModule : MonoBehaviour, IUnityContainerModule
    {
        [SerializeField]
        private MonoBehaviourWithBarDependency monoBehaviourWithBarDependency = default!;

        [SerializeField]
        private InitializableMonoBehaviour initializableMonoBehaviour = default!;

        [SerializeField]
        private AsyncInitializableMonoBehaviour asyncInitializableMonoBehaviour = default!;

        public void Configure(ContainerBuilder builder)
        {
            builder.RegisterInstance(monoBehaviourWithBarDependency);
            builder.RegisterInstance(initializableMonoBehaviour).AsImplementedInterfaces();
            builder.RegisterInstance(asyncInitializableMonoBehaviour).AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<Foo>().SingleInstance();
            builder.RegisterType<Bar>().AsSelf().As<IBar>().SingleInstance();
        }
    }
}