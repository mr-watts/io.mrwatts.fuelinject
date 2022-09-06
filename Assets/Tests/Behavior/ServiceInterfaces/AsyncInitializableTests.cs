using System.Collections;
using System.Threading.Tasks;
using Autofac;
using MrWatts.Internal.FuelInject.Testing;
using MrWatts.Internal.FuelInject.Testing.Utility;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace MrWatts.Internal.FuelInject.TestProject.Tests.Behaviour
{
    [TestFixture]
    internal sealed class AsyncInitializableTests : SceneTestGroup
    {
        [UnityTest]
        public IEnumerator SetupSceneWaitsForGameObjectsToInitializeIfRequested()
        {
            yield return SetupScene("TestScene", null, true);

            Assert.IsTrue(GameObjectFinder.Get<AsyncInitializableMonoBehaviour>().IsInitialized);
        }

        [UnityTest]
        public IEnumerator SetupSceneWaitsForAsyncServicesToInitializeIfRequested()
        {
            TaskCompletionSource<bool> completionSource = new();
            var asyncInitializable = new ConfigurableAsyncInitializable(completionSource.Task);

            IEnumerator sceneLoadingCoroutine = SetupScene(
                "TestScene",
                builder =>
                {
                    builder.RegisterInstance(asyncInitializable).As<IAsyncInitializable>();
                }
            );

            Assert.IsFalse(asyncInitializable.IsInitialized);

            completionSource.SetResult(true);

            yield return sceneLoadingCoroutine;

            Assert.IsTrue(asyncInitializable.IsInitialized);
        }

        [UnityTest]
        public IEnumerator SetupSceneDoesNotWaitForServicesToInitializeIfNotRequested()
        {
            TaskCompletionSource<bool> completionSource = new();
            var asyncInitializable = new ConfigurableAsyncInitializable(completionSource.Task);

            IEnumerator sceneLoadingCoroutine = SetupScene(
                "TestScene",
                builder =>
                {
                    builder.RegisterInstance(asyncInitializable).As<IAsyncInitializable>();
                },
                false
            );

            Assert.IsFalse(asyncInitializable.IsInitialized);

            yield return sceneLoadingCoroutine;

            Assert.IsFalse(asyncInitializable.IsInitialized);
        }
    }
}