using System;
using System.Collections;
using System.Collections.Generic;
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
        public IEnumerator ExecutesDisposablesInRequestedOrder()
        {
            var disposableFirst = new ListeningDisposable();
            var disposableDefault = new ListeningDisposable();
            var disposableMid = new ListeningDisposable();
            var disposableLast = new ListeningDisposable();

            var list = new List<IDisposable>();

            disposableFirst.OnDisposed += (_, _) => list.Add(disposableFirst);
            disposableDefault.OnDisposed += (_, _) => list.Add(disposableDefault);
            disposableMid.OnDisposed += (_, _) => list.Add(disposableMid);
            disposableLast.OnDisposed += (_, _) => list.Add(disposableLast);

            yield return SetupScene(
                "TestScene",
                builder =>
                {
                    builder.RegisterInstance(disposableMid).As<IDisposable>().WithOrder(500);
                    builder.RegisterInstance(disposableDefault).As<IDisposable>();
                    builder.RegisterInstance(disposableFirst).As<IDisposable>().WithOrder(-1000);
                    builder.RegisterInstance(disposableLast).As<IDisposable>().WithOrder(1000);
                }
            );

            yield return TearDownAllScenes();

            Assert.AreEqual(4, list.Count);
            Assert.AreSame(disposableFirst, list[0]);
            Assert.AreSame(disposableDefault, list[1]);
            Assert.AreSame(disposableMid, list[2]);
            Assert.AreSame(disposableLast, list[3]);
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