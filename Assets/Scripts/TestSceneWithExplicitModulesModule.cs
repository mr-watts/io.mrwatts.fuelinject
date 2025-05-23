using Autofac;
using UnityEngine;

namespace MrWatts.Internal.FuelInject.TestProject
{
    public sealed class TestSceneWithExplicitModulesModule : MonoBehaviour, IUnityContainerModule
    {
        [SerializeField]
        private MonoBehaviourWithBarDependency monoBehaviourWithBarDependency = default!;

        public void Configure(ContainerBuilder builder)
        {
            builder.RegisterInstance(monoBehaviourWithBarDependency);

            builder.RegisterType<Bar>().AsSelf().As<IBar>().SingleInstance();
        }
    }
}