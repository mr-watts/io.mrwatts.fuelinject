using System;
using System.Collections;
using System.Collections.Generic;
using Autofac;
using MrWatts.Internal.FuelInject.Testing;
using MrWatts.Internal.FuelInject.Testing.Utility;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace MrWatts.Internal.FuelInject.TestProject.Tests.Behaviour
{
    [TestFixture]
    internal sealed class InitializableTests : SceneTestGroup
    {
        [UnityTest]
        public IEnumerator SetupSceneWaitsForGameObjectsToInitializeIfRequested()
        {
            yield return SetupScene("TestScene", null, true);

            Assert.IsTrue(GameObjectFinder.Get<InitializableMonoBehaviour>().IsInitialized);
        }

        [UnityTest]
        public IEnumerator SetupSceneWaitsForServicesToInitializeIfRequested()
        {
            bool wasCalled = false;

            CallbackInvokingInitializable initializable = new(() => wasCalled = true);

            yield return SetupScene(
                "TestScene",
                builder =>
                {
                    builder.RegisterInstance(initializable).As<IInitializable>();
                }
            );

            Assert.IsTrue(wasCalled);
        }

        // NOTE: Can't be tested because the scene will already have initialized it anyway due to it being synchronous.
        // [UnityTest]
        // public IEnumerator SetupSceneDoesNotWaitForServicesToInitializeIfNotRequested()
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
        public IEnumerator ExecutesInitializablesInRequestedOrder()
        {
            var initializableFirst = new ListeningInitializable();
            var initializableDefault = new ListeningInitializable();
            var initializableMid = new ListeningInitializable();
            var initializableLast = new ListeningInitializable();

            var list = new List<IInitializable>();

            initializableFirst.OnInitialized += (_, _) => list.Add(initializableFirst);
            initializableDefault.OnInitialized += (_, _) => list.Add(initializableDefault);
            initializableMid.OnInitialized += (_, _) => list.Add(initializableMid);
            initializableLast.OnInitialized += (_, _) => list.Add(initializableLast);

            yield return SetupScene(
                "TestScene",
                builder =>
                {
                    builder.RegisterInstance(initializableMid).As<IInitializable>().WithOrder(500);
                    builder.RegisterInstance(initializableDefault).As<IInitializable>();
                    builder.RegisterInstance(initializableFirst).As<IInitializable>().WithOrder(-1000);
                    builder.RegisterInstance(initializableLast).As<IInitializable>().WithOrder(1000);
                }
            );

            Assert.AreEqual(4, list.Count);
            Assert.AreSame(initializableFirst, list[0]);
            Assert.AreSame(initializableDefault, list[1]);
            Assert.AreSame(initializableMid, list[2]);
            Assert.AreSame(initializableLast, list[3]);
        }

        [UnityTest]
        public IEnumerator ExceptionsAreSentToUnityKernelLogger()
        {
            bool didThrowException = false;
            var initializable = new CallbackInvokingInitializable(
                () =>
                {
                    didThrowException = true;

                    throw new Exception("Oh no, initializable failed");
                }
            );
            var listeningUnityKernelLogger = new ListeningUnityKernelLogger();

            IEnumerator sceneLoadingCoroutine = SetupScene(
                "TestScene",
                builder =>
                {
                    builder.RegisterInstance(initializable).As<IInitializable>();
                    builder.RegisterInstance(listeningUnityKernelLogger).As<IUnityKernelLogger>();
                },
                false
            );

            Assert.IsFalse(listeningUnityKernelLogger.WasExceptionLogged);

            // Need to do it this way since our failing initializable will prevent the final initializable SetupScene
            // registers to know when they all finished from running, so we would be blocking indefinitely.
            while (!didThrowException)
            {
                yield return sceneLoadingCoroutine;
            }

            yield return new WaitTemporarilyUntil(() => listeningUnityKernelLogger.WasExceptionLogged);

            Assert.IsTrue(listeningUnityKernelLogger.WasExceptionLogged);
        }
    }
}