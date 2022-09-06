using System.Collections;
using System.Collections.Concurrent;
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

        [UnityTest]
        public IEnumerator ExecutesInitializablesInRequestedOrder()
        {
            var asyncInitializableFirst = new ConfigurableAsyncInitializable(Task.CompletedTask);
            var asyncInitializableDefault = new ConfigurableAsyncInitializable(Task.CompletedTask);
            var asyncInitializableMid = new ConfigurableAsyncInitializable(Task.CompletedTask);
            var asyncInitializableLast = new ConfigurableAsyncInitializable(Task.CompletedTask);

            var queue = new ConcurrentQueue<IAsyncInitializable>();

            asyncInitializableFirst.OnInitialized += (_, _) => queue.Enqueue(asyncInitializableFirst);
            asyncInitializableDefault.OnInitialized += (_, _) => queue.Enqueue(asyncInitializableDefault);
            asyncInitializableMid.OnInitialized += (_, _) => queue.Enqueue(asyncInitializableMid);
            asyncInitializableLast.OnInitialized += (_, _) => queue.Enqueue(asyncInitializableLast);

            yield return SetupScene(
                "TestScene",
                builder =>
                {
                    builder.RegisterInstance(asyncInitializableMid).As<IAsyncInitializable>().WithOrder(500);
                    builder.RegisterInstance(asyncInitializableDefault).As<IAsyncInitializable>();
                    builder.RegisterInstance(asyncInitializableFirst).As<IAsyncInitializable>().WithOrder(-1000);
                    builder.RegisterInstance(asyncInitializableLast).As<IAsyncInitializable>().WithOrder(1000);
                }
            );

            IAsyncInitializable initializable;

            Assert.True(queue.TryDequeue(out initializable));
            Assert.AreSame(asyncInitializableFirst, initializable);

            Assert.True(queue.TryDequeue(out initializable));
            Assert.AreSame(asyncInitializableDefault, initializable);

            Assert.True(queue.TryDequeue(out initializable));
            Assert.AreSame(asyncInitializableMid, initializable);

            Assert.True(queue.TryDequeue(out initializable));
            Assert.AreSame(asyncInitializableLast, initializable);
        }
    }
}