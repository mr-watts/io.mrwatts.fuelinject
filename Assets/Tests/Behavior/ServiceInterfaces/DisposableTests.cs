using System;
using System.Collections;
using Autofac;
using MrWatts.Internal.FuelInject.Testing;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace MrWatts.Internal.FuelInject.TestProject.Tests.Behaviour
{
    [TestFixture]
    internal sealed class DisposableTests : SceneTestGroup
    {
        [UnityTest]
        public IEnumerator ExecutesDisposables()
        {
            bool wasCalled = false;

            CallbackInvokingDisposable tickable = new(() => wasCalled = true);

            yield return SetupScene(
                "TestScene",
                builder =>
                {
                    builder.RegisterInstance(tickable).As<IDisposable>();
                }
            );

            Assert.IsFalse(wasCalled);

            yield return TearDownAllScenes();

            Assert.IsTrue(wasCalled);
        }

        [UnityTest]
        public IEnumerator ExceptionsAreSentToUnityKernelLogger()
        {
            CallbackInvokingDisposable disposable = new(() => throw new Exception("Oh no, disposable failed"));
            ListeningUnityKernelLogger listeningUnityKernelLogger = new();

            IEnumerator sceneLoadingCoroutine = SetupScene(
                "TestScene",
                builder =>
                {
                    builder.RegisterInstance(disposable).As<IDisposable>();
                    builder.RegisterInstance(listeningUnityKernelLogger).As<IUnityKernelLogger>();
                }
            );

            yield return sceneLoadingCoroutine;

            Assert.IsFalse(listeningUnityKernelLogger.WasExceptionLogged);

            yield return TearDownAllScenes();

            Assert.IsTrue(listeningUnityKernelLogger.WasExceptionLogged);
        }
    }
}