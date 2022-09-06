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
            var initializable = new ListeningInitializable();

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
    }
}