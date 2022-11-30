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
    internal sealed class FixedTickableTests : SceneTestGroup
    {
        [UnityTest]
        public IEnumerator ExecutesFixedTickables()
        {
            bool wasCalled = false;

            CallbackInvokingFixedTickable tickable = new(() => wasCalled = true);

            yield return SetupScene(
                "TestScene",
                builder =>
                {
                    builder.RegisterInstance(tickable).As<IFixedTickable>();
                }
            );

            yield return new WaitTemporarilyUntil(() => wasCalled);
        }

        [UnityTest]
        public IEnumerator ExceptionsAreSentToUnityKernelLogger()
        {
            CallbackInvokingFixedTickable tickable = new(() => throw new Exception("Oh no, tickable failed"));
            ListeningUnityKernelLogger listeningUnityKernelLogger = new();

            IEnumerator sceneLoadingCoroutine = SetupScene(
                "TestScene",
                builder =>
                {
                    builder.RegisterInstance(tickable).As<IFixedTickable>();
                    builder.RegisterInstance(listeningUnityKernelLogger).As<IUnityKernelLogger>();
                },
                false
            );

            Assert.IsFalse(listeningUnityKernelLogger.WasExceptionLogged);

            yield return sceneLoadingCoroutine;
            yield return new WaitTemporarilyUntil(() => listeningUnityKernelLogger.WasExceptionLogged);
        }
    }
}