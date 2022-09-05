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
    internal sealed class ContainerBuilderOverrideTests : SceneTestGroup
    {
        [UnityTest]
        public IEnumerator ContainerBindingsCanBeOverriddenDuringTests()
        {
            bool isOverrideInvoked = false;

            yield return SetupScene(
                "TestScene",
                builder =>
                {
                    isOverrideInvoked = true;

                    builder.RegisterType<CustomBar>().As<IBar>().SingleInstance();
                }
            );

            Assert.IsTrue(isOverrideInvoked, "Registered container module override handlers are not invoked");

            var service = GameObjectFinder.Get<MonoBehaviourWithBarDependency>();

            Assert.IsInstanceOf(
                typeof(CustomBar),
                service.BarGetter,
                "Container module override handlers do not properly override existing bindings. Perhaps they " +
                "aren't loaded after the actual scene modules and are thus overridden again by them?"
            );
        }

        [UnityTest]
        public IEnumerator SetupSceneWaitsForInitializableGameObjectsToExecuteIfRequested()
        {
            yield return SetupScene("TestScene", null, true);

            Assert.IsTrue(GameObjectFinder.Get<InitializableMonoBehaviour>().IsInitialized);
        }

        [UnityTest]
        public IEnumerator SetupSceneWaitsForInitializableServicesToExecuteIfRequested()
        {
            var initializable = new ConfigurableInitializable();

            yield return SetupScene(
                "TestScene",
                builder =>
                {
                    builder.RegisterInstance(initializable).As<IInitializable>();
                }
            );

            Assert.IsTrue(initializable.IsInitialized);
        }

        // NOTE: Can't be tested because the scene will already have initialized it anyway due to it being synchronous.
        // [UnityTest]
        // public IEnumerator SetupSceneDoesNotWaitForInitializableServicesToExecuteIfNotRequested()
        // {
        //     var initializable = new ConfigurableInitializable();

        //     yield return SetupScene(
        //         "TestScene",
        //         builder =>
        //         {
        //             builder.RegisterInstance(initializable).As<IInitializable>();
        //         },
        //         false
        //     );

        //     Assert.IsFalse(initializable.IsInitialized);
        // }

        [UnityTest]
        public IEnumerator SetupSceneWaitsForAsyncInitializableGameObjectsToExecuteIfRequested()
        {
            yield return SetupScene("TestScene", null, true);

            Assert.IsTrue(GameObjectFinder.Get<AsyncInitializableMonoBehaviour>().IsInitialized);
        }

        [UnityTest]
        public IEnumerator SetupSceneWaitsForAsyncInitializableServicesToExecuteIfRequested()
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
        public IEnumerator SetupSceneDoesNotWaitForAsyncInitializableServicesToExecuteIfNotRequested()
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