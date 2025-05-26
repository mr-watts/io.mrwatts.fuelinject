using System.Collections;
using Autofac;
using MrWatts.Internal.FuelInject.Testing;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace MrWatts.Internal.FuelInject.TestProject.Tests.Behaviour
{
    [TestFixture]
    internal sealed class ServiceRetrieverTests : SceneTestGroup
    {
        [UnityTest]
        public IEnumerator ServicesFromContainerCanBeRetrievedDuringTestsForConvenience()
        {
            yield return SetupScene("TestScene");

            Assert.NotNull(Container.ResolveOptional<IBar>());
        }
    }
}