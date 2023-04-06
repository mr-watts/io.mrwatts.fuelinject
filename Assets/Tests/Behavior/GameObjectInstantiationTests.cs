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
    internal sealed class GameObjectInstantiationTests : SceneTestGroup
    {
        [UnityTest]
        public IEnumerator GameObjectInstantiatorCanBeUsedToDynamicallySpawnObjects()
        {
            yield return LoadTestScene();

            var service = GameObjectFinder.Get<MonoBehaviourWithGameObjectInstantiatorDependency>();

            GameObject gameObject = service.GenericInstantiator.Instantiate();

            Assert.NotNull(gameObject);
        }

        [UnityTest]
        public IEnumerator GameObjectInstantiatorCanBeUsedToCloneObjectsAndAutomaticallyInject()
        {
            yield return LoadTestScene();

            var service = GameObjectFinder.Get<MonoBehaviourWithGameObjectInstantiatorDependency>();

            GameObject originalObject = GameObjectFinder.Get<MonoBehaviourWithBarDependency>().gameObject;
            GameObject gameObject = service.SpecificInstantiator.Instantiate(originalObject);

            Assert.NotNull(gameObject.GetComponent<MonoBehaviourWithBarDependency>());
            Assert.IsInstanceOf(typeof(IBar), gameObject.GetComponent<MonoBehaviourWithBarDependency>().BarGetter);

            // Tests use additive scenes, so ensure the newly spawned game object is in the same scene as the original
            // one, or test tear down won't clean it up as it is in the root scene by default.
            SceneManager.MoveGameObjectToScene(gameObject, originalObject.scene);
        }

        private IEnumerator LoadTestScene()
        {
            yield return SetupScene("TestScene");
        }
    }
}