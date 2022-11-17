using System;
using System.Collections;
using Autofac;
using MrWatts.Internal.FuelInject.Testing;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace MrWatts.Internal.FuelInject.TestProject.Tests.Behaviour
{
    [TestFixture]
    internal sealed class LateTickableTests : SceneTestGroup
    {
        [UnityTest]
        public IEnumerator ExecutesLateTickables()
        {
            bool wasCalled = false;

            CallbackInvokingLateTickable tickable = new(() => wasCalled = true);

            yield return SetupScene(
                "TestScene",
                builder =>
                {
                    builder.RegisterInstance(tickable).As<ILateTickable>();
                }
            );

            yield return new WaitForEndOfFrame();

            Assert.IsTrue(wasCalled);
        }

        [UnityTest]
        public IEnumerator ExceptionsAreSentToUnityKernelLogger()
        {
            CallbackInvokingLateTickable tickable = new(() => throw new Exception("Oh no, tickable failed"));
            ListeningUnityKernelLogger listeningUnityKernelLogger = new();

            IEnumerator sceneLoadingCoroutine = SetupScene(
                "TestScene",
                builder =>
                {
                    builder.RegisterInstance(tickable).As<ILateTickable>();
                    builder.RegisterInstance(listeningUnityKernelLogger).As<IUnityKernelLogger>();
                },
                false
            );

            Assert.IsFalse(listeningUnityKernelLogger.WasExceptionLogged);

            yield return sceneLoadingCoroutine;
            yield return new WaitForEndOfFrame();

            Assert.IsTrue(listeningUnityKernelLogger.WasExceptionLogged);
        }
    }
}