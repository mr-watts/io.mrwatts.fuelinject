using Autofac;
using UnityEngine;

namespace MrWatts.Internal.FuelInject.TestProject
{
    public sealed class TestSceneModule : MonoBehaviour, IUnityContainerModule
    {
        [SerializeField]
        private FooMonoBehaviour fooMonoBehaviour = default!;

        public void Configure(ContainerBuilder builder)
        {
            builder.RegisterInstance<FooMonoBehaviour>(fooMonoBehaviour);

            builder.RegisterType<Foo>().SingleInstance();
            builder.RegisterType<Bar>().SingleInstance();

        }
    }
}