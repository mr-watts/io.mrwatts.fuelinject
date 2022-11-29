using System;
using System.Collections;
using Autofac;
using MrWatts.Internal.FuelInject.Testing;
using MrWatts.Internal.FuelInject.Testing.Utility;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace MrWatts.Internal.FuelInject.TestProject.Tests.Behaviour
{
    [TestFixture]
    internal sealed class TickableTests : SceneTestGroup
    {
        [UnityTest]
        public IEnumerator ExecutesTickables()
        {
            bool wasCalled = false;

            CallbackInvokingTickable tickable = new(() => wasCalled = true);

            yield return SetupScene(
                "TestScene",
                builder =>
                {
                    builder.RegisterInstance(tickable).As<ITickable>();
                }
            );

            yield return new WaitTemporarilyUntil(() => wasCalled = true);

            Assert.IsTrue(wasCalled);
        }

        [UnityTest]
        public IEnumerator ExceptionsAreSentToUnityKernelLogger()
        {
            CallbackInvokingTickable tickable = new(() => throw new Exception("Oh no, tickable failed"));
            ListeningUnityKernelLogger listeningUnityKernelLogger = new();

            IEnumerator sceneLoadingCoroutine = SetupScene(
                "TestScene",
                builder =>
                {
                    builder.RegisterInstance(tickable).As<ITickable>();
                    builder.RegisterInstance(listeningUnityKernelLogger).As<IUnityKernelLogger>();
                },
                false
            );

            Assert.IsFalse(listeningUnityKernelLogger.WasExceptionLogged);

            yield return sceneLoadingCoroutine;
            yield return new WaitTemporarilyUntil(() => listeningUnityKernelLogger.WasExceptionLogged);

            Assert.IsTrue(listeningUnityKernelLogger.WasExceptionLogged);
        }
    }
}