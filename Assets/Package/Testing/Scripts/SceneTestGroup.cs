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
            SceneSetupResult result = SetupScene(new(name)
            {
                ContainerBindingCallback = containerBindingCallback,
                AttachKernelListeners = waitForInitializables,
            });

            yield return result.SceneLoadingOperation;

            if (waitForInitializables)
            {
                yield return new WaitForAsyncResult(result.InitializableTask);
                yield return new WaitForAsyncResult(result.AsyncInitializableTask);
            }
        }

        protected SceneSetupResult SetupScene(SceneSetupParameters parameters)
        {
            TaskCompletionSource<bool> initializableSource = new();
            TaskCompletionSource<bool> asyncInitializableSource = new();

            if (parameters.AttachKernelListeners)
            {
                RegisterContainerOverrideHandler(builder =>
                {
                    builder
                        .Register(_ => new CallbackInvokingInitializable(() => initializableSource.SetResult(true)))
                        .As<IInitializable>()
                        .SingleInstance();

                    builder
                        .Register(_ => new CallbackInvokingAsyncInitializable(() => asyncInitializableSource.SetResult(true)))
                        .As<IAsyncInitializable>()
                        .SingleInstance();
                });
            }

            if (parameters.ContainerBindingCallback is not null)
            {
                RegisterContainerOverrideHandler(parameters.ContainerBindingCallback);
            }

            return new(
                SceneLoader.Load(parameters.SceneName, true),
                initializableSource.Task,
                asyncInitializableSource.Task
            );
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