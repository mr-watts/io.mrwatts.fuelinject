using System.Collections;
using Autofac;
using MrWatts.Internal.FuelInject.Testing;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace MrWatts.Internal.FuelInject.TestProject.Tests.Behaviour
{
    [TestFixture]
    internal sealed class ContainerForwardingTests : SceneTestGroup
    {
        [UnityTest]
        public IEnumerator ServicesNotPresentInMainContainerFallBackToOtherContainersInScenes()
        {
            yield return LoadTestScene();

            Assert.True(
                Container.TryResolve<FallbackService>(out var _),
                $"Service that is not present in main container but is in another container in the scene and registered through the {nameof(ContainerForwardingModule)} was not found"
            );
        }

        private IEnumerator LoadTestScene()
        {
            yield return SetupScene("FallbackContainerScene");
            yield return SetupScene("TestScene");
        }
    }
}