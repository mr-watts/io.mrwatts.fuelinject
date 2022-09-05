using Autofac;
using UnityEngine;

namespace MrWatts.Internal.FuelInject.TestProject
{
    public sealed class TestSceneModule : MonoBehaviour, IUnityContainerModule
    {
        [SerializeField]
        private MonoBehaviourWithBarDependency monoBehaviourWithBarDependency = default!;

        public void Configure(ContainerBuilder builder)
        {
            builder.RegisterInstance(monoBehaviourWithBarDependency);

            builder.RegisterType<Foo>().SingleInstance();
            builder.RegisterType<Bar>().SingleInstance();
        }
    }
}