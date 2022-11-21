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
        [UnityTest]
        public IEnumerator MonoBehaviourPropertyWithInjectAttributeIsInjected()
        {
            yield return LoadTestScene();

            var service = GameObjectFinder.Get<MonoBehaviourWithBarDependency>();

            Assert.NotNull(service.BarGetter);
            Assert.IsInstanceOf(typeof(Bar), service.BarGetter);
        }

        [UnityTest]
        public IEnumerator MonoBehaviourPropertyOfTypeIInjectorObjectWithInjectAttributeIsInjected()
        {
            yield return LoadTestScene();

            var service = GameObjectFinder.Get<MonoBehaviourWithIInjectorObjectDependency>();

            Assert.NotNull(service.InjectorGetter);
            Assert.IsInstanceOf(typeof(IInjector<object>), service.InjectorGetter);
        }

        [UnityTest]
        public IEnumerator MonoBehaviourPropertyOfTypeIInjectorGameObjectWithInjectAttributeIsInjected()
        {
            yield return LoadTestScene();

            var service = GameObjectFinder.Get<MonoBehaviourWithIInjectorGameObjectDependency>();

            Assert.NotNull(service.InjectorGetter);
            Assert.IsInstanceOf(typeof(IInjector<GameObject>), service.InjectorGetter);
        }

        [UnityTest]
        public IEnumerator MonoBehaviourPropertyOfTypeIInjectorSceneWithInjectAttributeIsInjected()
        {
            yield return LoadTestScene();

            var service = GameObjectFinder.Get<MonoBehaviourWithIInjectorSceneDependency>();

            Assert.NotNull(service.InjectorGetter);
            Assert.IsInstanceOf(typeof(IInjector<Scene>), service.InjectorGetter);
        }

        [UnityTest]
        public IEnumerator GameObjectInjectorCanBeUsedToInjectIntoDynamicallySpawnedObjects()
        {
            yield return LoadTestScene();

            var service = GameObjectFinder.Get<MonoBehaviourWithIInjectorGameObjectDependency>();

            GameObject gameObject = new();
            gameObject.AddComponent<MonoBehaviourWithIInjectorGameObjectDependency>();

            var component = gameObject.GetComponent<MonoBehaviourWithIInjectorGameObjectDependency>();

            Assert.IsNull(component.InjectorGetter);

            service.InjectorGetter.Inject(gameObject);

            Assert.NotNull(component.InjectorGetter);
        }

        private IEnumerator LoadTestScene()
        {
            yield return SetupScene("TestScene");
        }
    }
}