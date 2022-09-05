using System.Collections;
using MrWatts.Internal.FuelInject.Testing;
using MrWatts.Internal.FuelInject.Testing.Utility;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace MrWatts.Internal.FuelInject.TestProject.Tests.Behaviour
{
    /// <summary>
    /// <para>Contains tests that verify the scene is cleaned up properly after running a test.</para>
    /// <para>The prefixes are necessary to force Unity to run them in the right order.</para>
    /// </summary>
    [TestFixture]
    internal sealed class SceneCleanupTests : SceneTestGroup
    {
        [UnityTest]
        [Order(1)]
        public IEnumerator SceneTestProperlyCleansUpGameObjectsPreparation()
        {
            yield return LoadTestScene();

            var service = GameObjectFinder.Get<MonoBehaviourWithBarDependency>();

            Assert.NotNull(service!.BarGetter);
        }

        [UnityTest]
        [Order(2)]
        public IEnumerator SceneTestProperlyCleansUpGameObjects()
        {
            yield return null;

            var fooMonoBehaviour = GameObjectFinder.Find<MonoBehaviourWithBarDependency>();

            Assert.IsNull(
                fooMonoBehaviour,
                "Foo exists in a test that never loaded a scene. If you are running all tests in succession " +
                "and this fails, this means another scene test is leaking its scene objects and not clearing them properly."
            );
        }

        /*
            Can't cause a test to fail on purpose as ExceptedException no longer exists in NUnit, and it can't be
            implemented ourselves since it requires implementing IEnumerableTestMethodCommand, which is internal.
        */
        // [UnityTest]
        // [Order(3)]
        // public IEnumerator SceneTestProperlyCleansUpGameObjectsWhenExceptionOccursPreparation()
        // {
        //     yield return LoadTestScene();

        //     var service = GameObjectFinder.Get<MonoBehaviourWithBarDependency>();

        //     Assert.NotNull(service!.BarGetter);

        //     throw new Exception("Oh no, the test failed");
        // }

        // [UnityTest]
        // [Order(4)]
        // public IEnumerator SceneTestProperlyCleansUpGameObjectsWhenExceptionOccurs()
        // {
        //     yield return null;

        //     var fooMonoBehaviour = GameObjectFinder.Find<MonoBehaviourWithBarDependency>();

        //     Assert.IsNull(
        //         fooMonoBehaviour,
        //         "Foo exists in a test that never loaded a scene. If you are running all tests in succession " +
        //         "and this fails, this means another scene test is leaking its scene objects and not clearing them " +
        //         "properly when it fails with an exception."
        //     );
        // }

        private IEnumerator LoadTestScene()
        {
            yield return SetupScene("TestScene");
        }
    }
}