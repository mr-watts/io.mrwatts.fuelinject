using System.Collections;
using MrWatts.Internal.FuelInject.Testing;
using MrWatts.Internal.FuelInject.Testing.Utility;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace MrWatts.Internal.FuelInject.TestProject.Tests.Behaviour
{
    [TestFixture]
    internal sealed class MonoBehaviourDependencyInjectionTests : SceneTestGroup
    {
        protected override string? SceneName { get; } = "TestScene";

        [UnityTest]
        public IEnumerator MonoBehaviourPropertyWithInjectAttributeIsInjected()
        {
            var service = GameObjectFinder.Get<MonoBehaviourWithBarDependency>();

            Assert.NotNull(service!.BarGetter);
            Assert.IsInstanceOf(typeof(Bar), service.BarGetter);

            yield return null;
        }

        [UnityTest]
        public IEnumerator MonoBehaviourPropertyOfTypeIInjectorObjectWithInjectAttributeIsInjected()
        {
            var service = GameObjectFinder.Get<MonoBehaviourWithIInjectorObjectDependency>();

            Assert.NotNull(service.InjectorGetter);
            Assert.IsInstanceOf(typeof(IInjector<object>), service.InjectorGetter);

            yield return null;
        }

        [UnityTest]
        public IEnumerator MonoBehaviourPropertyOfTypeIInjectorGameObjectWithInjectAttributeIsInjected()
        {
            var service = GameObjectFinder.Get<MonoBehaviourWithIInjectorGameObjectDependency>();

            Assert.NotNull(service.InjectorGetter);
            Assert.IsInstanceOf(typeof(IInjector<GameObject>), service.InjectorGetter);

            yield return null;
        }

        [UnityTest]
        public IEnumerator MonoBehaviourPropertyOfTypeIInjectorSceneWithInjectAttributeIsInjected()
        {
            var service = GameObjectFinder.Get<MonoBehaviourWithIInjectorSceneDependency>();

            Assert.NotNull(service.InjectorGetter);
            Assert.IsInstanceOf(typeof(IInjector<Scene>), service.InjectorGetter);

            yield return null;
        }
    }
}