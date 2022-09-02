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
            // TODO: Add proper tests.
            UnityEngine.Debug.Log($"Test module configuring container");

            builder.RegisterInstance<FooMonoBehaviour>(fooMonoBehaviour);

            builder.RegisterType<Foo>().SingleInstance();
            builder.RegisterType<Bar>().SingleInstance();
        }
    }
}