using System.Collections;
using Autofac;
using MrWatts.Internal.FuelInject.Testing;
using MrWatts.Internal.FuelInject.Testing.Utility;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace MrWatts.Internal.FuelInject.TestProject.Tests.Behaviour
{
    [TestFixture]
    internal sealed class ContainerBuilderOverrideTests : SceneTestGroup
    {
        [UnityTest]
        public IEnumerator ContainerBindingsCanBeOverriddenDuringTestsWithAutomaticallyAddRootGameObjectModulesEnabled()
        {
            bool isOverrideInvoked = false;

            yield return SetupScene(
                "TestScene",
                builder =>
                {
                    isOverrideInvoked = true;

                    builder.RegisterType<CustomBar>().As<IBar>().SingleInstance();
                }
            );

            Assert.IsTrue(isOverrideInvoked, "Registered container module override handlers are not invoked");

            var service = GameObjectFinder.Get<MonoBehaviourWithBarDependency>();

            Assert.IsInstanceOf(
                typeof(CustomBar),
                service.BarGetter,
                "Container module override handlers do not properly override existing bindings. Perhaps they " +
                "aren't loaded after the actual scene modules and are thus overridden again by them?"
            );
        }

        [UnityTest]
        public IEnumerator ContainerBindingsCanBeOverriddenDuringTestsWithAutomaticallyAddRootGameObjectModulesDisabled()
        {
            bool isOverrideInvoked = false;

            yield return SetupScene(
                "TestSceneExplicitModules",
                builder =>
                {
                    isOverrideInvoked = true;

                    builder.RegisterType<CustomBar>().As<IBar>().SingleInstance();
                }
            );

            Assert.IsTrue(isOverrideInvoked, "Registered container module override handlers are not invoked");

            var service = GameObjectFinder.Get<MonoBehaviourWithBarDependency>();

            Assert.IsInstanceOf(
                typeof(CustomBar),
                service.BarGetter,
                "Container module override handlers do not properly override existing bindings. Perhaps they " +
                "aren't loaded after the actual scene modules and are thus overridden again by them?"
            );
        }

        [UnityTest]
        public IEnumerator GlobalOverridingModuleIsOnlyRegisteredOnceWithAutomaticallyAddRootGameObjectModulesEnabled()
        {
            int timesOverrideInvoked = 0;

            yield return SetupScene(
                "TestScene",
                _ =>
                {
                    ++timesOverrideInvoked;
                }
            );

            Assert.AreEqual(1, timesOverrideInvoked, "Global overriding modules should only be registered once");
        }
    }
}