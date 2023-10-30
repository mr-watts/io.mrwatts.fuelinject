using System;
using System.Collections;
using System.Threading.Tasks;
using Autofac;
using MrWatts.Internal.FuelInject.Testing;
using MrWatts.Internal.FuelInject.Testing.Utility;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace MrWatts.Internal.FuelInject.TestProject.Tests.Behaviour
{
    [TestFixture]
    internal sealed class AsyncTickableTests : SceneTestGroup
    {
        [UnityTest]
        public IEnumerator ExecutesAsyncTickablesEachFrame()
        {
            var asyncTickable = new ConfigurableAsyncTickable(Task.CompletedTask);

            IEnumerator sceneLoadingCoroutine = SetupScene(
                "TestScene",
                builder =>
                {
                    builder.RegisterInstance(asyncTickable).As<IAsyncTickable>();
                }
            );

            Assert.IsFalse(asyncTickable.HasTicked);

            yield return sceneLoadingCoroutine;
            yield return new WaitForEndOfFrame();

            Assert.IsTrue(asyncTickable.HasTicked);

            asyncTickable.Reset();

            yield return new WaitForEndOfFrame();

            Assert.IsTrue(asyncTickable.HasTicked);
        }

        [UnityTest]
        public IEnumerator OneCanBlockAllAsyncTickablesUntilFullyCompleted()
        {
            TaskCompletionSource<bool> completionSource = new();
            var blockingAsyncTickable = new ConfigurableAsyncTickable(completionSource.Task);
            var workingAsyncTickable = new ConfigurableAsyncTickable(Task.CompletedTask);

            IEnumerator sceneLoadingCoroutine = SetupScene(
                "TestScene",
                builder =>
                {
                    builder.RegisterInstance(blockingAsyncTickable).As<IAsyncTickable>();
                    builder.RegisterInstance(workingAsyncTickable).As<IAsyncTickable>();
                }
            );

            yield return sceneLoadingCoroutine;
            yield return new WaitForEndOfFrame();

            Assert.IsFalse(workingAsyncTickable.HasTicked);
            Assert.IsFalse(blockingAsyncTickable.HasTicked);

            yield return new WaitForEndOfFrame();

            Assert.IsFalse(workingAsyncTickable.HasTicked);
            Assert.IsFalse(blockingAsyncTickable.HasTicked);

            completionSource.SetResult(true);

            yield return new WaitForEndOfFrame();

            Assert.IsTrue(workingAsyncTickable.HasTicked);
            Assert.IsTrue(blockingAsyncTickable.HasTicked);
        }

        [UnityTest]
        public IEnumerator ExceptionsAreSentToUnityKernelLogger()
        {
            TaskCompletionSource<bool> completionSource = new();
            var asyncTickable = new ConfigurableAsyncTickable(completionSource.Task);

            ListeningUnityKernelLogger listeningUnityKernelLogger = new();

            IEnumerator sceneLoadingCoroutine = SetupScene(
                "TestScene",
                builder =>
                {
                    builder.RegisterInstance(asyncTickable).As<IAsyncTickable>();
                    builder.RegisterInstance(listeningUnityKernelLogger).As<IUnityKernelLogger>();
                },
                false
            );

            Assert.IsFalse(listeningUnityKernelLogger.WasExceptionLogged);

            yield return sceneLoadingCoroutine;

            completionSource.SetException(new Exception("Oh no, async tickable failed"));

            yield return new WaitTemporarilyUntil(() => listeningUnityKernelLogger.WasExceptionLogged);
        }
    }
}