using System;
using System.Collections;
using System.Threading.Tasks;
using Autofac;
using MrWatts.Internal.FuelInject.Testing.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace MrWatts.Internal.FuelInject.Testing
{
    /// <summary>
    /// Represents a group of scene tests. Extend this class to make writing scene tests more convenient.
    /// </summary>
    public abstract class SceneTestGroup
    {
        protected SceneLoader SceneLoader { get; } = new();
        protected SceneUnloader SceneUnloader { get; } = new();

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            /*
                Normally unloading all scenes in TearDown should be sufficient, but there appears to be a "bug" in Unity
                2021.3.8 where the SceneManager omits scenes loaded during tests in TearDown that are still there at the
                end of the test, and are (strangely enough) back again when the _next_ test starts, resulting in test
                leakage.
            */
            yield return SceneUnloader.UnloadAll();
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return SceneUnloader.UnloadAll();
        }

        /// <summary>
        /// <para>Convenience wrapper for SceneLoader.Load that ensures a callback that is invoked when container bindings are registered in modules.</para>
        /// <para>This works by simply creating a fake container module and ensuring it is part of another scene, loading the requested scene additively to it.</para>
        /// <para>You *must* call this **before** loading your actual scene, and the ContainerModuleLoader in your scene **must** have automaticallyAddRootGameObjectModules set!</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="containerBindingCallback"></param>
        /// <param name="waitForInitializables">Whether or not to wait for all initializables to finish executing before proceeding with the test.</param>
        protected IEnumerator SetupScene(string name, Action<ContainerBuilder>? containerBindingCallback = null, bool waitForInitializables = true)
        {
            yield return SetupScene(
                new SceneSetupParameters(name)
                {
                    ContainerBindingCallback = containerBindingCallback,
                    WaitForInitializables = waitForInitializables,
                }
            );
        }

        protected IEnumerator SetupScene(SceneSetupParameters parameters)
        {
            if (parameters.ContainerBindingCallback is not null)
            {
                RegisterContainerOverrideHandler(parameters.ContainerBindingCallback);
            }

            SceneSetupResult result = new(SceneLoader.Load(parameters.SceneName, true));

            yield return result.SceneLoadingOperation;

            if (parameters.WaitForInitializables)
            {
                UnityKernel[] kernels = UnityEngine.Object.FindObjectsByType<UnityKernel>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);

                foreach (UnityKernel kernel in kernels)
                {
                    yield return new WaitForAsyncResult(kernel.InitializationTask);
                }
            }
        }

        /// <summary>
        /// <para>Registers a callback that is invoked when container bindings are registered in modules.</para>
        /// <para>This works by simply creating a fake container module and ensuring it is part of the scene.</para>
        /// <para>You *must* call this **before** loading your actual scene, and the ContainerModuleLoader in your scene **must** have automaticallyAddRootGameObjectModules set!</para>
        /// </summary>
        /// <param name="callback">Callback that takes a container, overriding its bindings.</param>
        private void RegisterContainerOverrideHandler(Action<ContainerBuilder> callback)
        {
            Scene scene = SceneManager.CreateScene($"FuelInjectTestContainerOverride-{Guid.NewGuid()}");

            GameObject gameObject = new(nameof(CallbackInvokingModule));

            CallbackInvokingModule module = gameObject.AddComponent<CallbackInvokingModule>();
            module.Priority = int.MaxValue; // Ensure we are always last.
            module.Callback = callback;

            SceneManager.MoveGameObjectToScene(gameObject, scene);
        }
    }
}