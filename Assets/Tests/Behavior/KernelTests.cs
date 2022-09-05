using System.Collections;
using MrWatts.Internal.FuelInject.Testing;
using MrWatts.Internal.FuelInject.Testing.Utility;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace MrWatts.Internal.FuelInject.TestProject.Tests.Behaviour
{
    [TestFixture]
    internal sealed class KernelTests : SceneTestGroup
    {
        [UnityTest]
        public IEnumerator InitializableServicesGetExecuted()
        {
            yield return SetupScene("TestScene");

            var service = GameObjectFinder.Get<InitializableMonoBehaviour>();

            Assert.IsTrue(service.IsInitialized);

            yield return null;
        }
    }
}