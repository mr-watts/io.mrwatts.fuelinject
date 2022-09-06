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
    }
}