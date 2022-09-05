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
        protected override string? SceneName { get; } = "TestScene";

        [UnityTest]
        public IEnumerator InitializableServicesGetExecuted()
        {
            var service = GameObjectFinder.Get<InitializableMonoBehaviour>();

            Assert.IsTrue(service.IsInitialized);

            yield return null;
        }
    }
}