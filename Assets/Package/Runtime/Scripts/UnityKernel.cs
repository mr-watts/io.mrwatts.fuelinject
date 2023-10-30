using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace MrWatts.Internal.FuelInject
{
    /// <summary>
    /// Thin MonoBehaviour that handles invoking the necessary Unity-related implementors of classes such as
    /// IInitializable, ITickable, and so on.
    /// </summary>
    public sealed class UnityKernel : MonoBehaviour
    {
        [Inject]
        private IInitializable? Initializable { get; set; }

        [Inject]
        private IAsyncInitializable? AsyncInitializable { get; set; }

        [Inject]
        private ITickable? Tickable { get; set; }

        [Inject]
        private IEnumerable<IAsyncTickable>? AsyncTickables { get; set; }

        [Inject]
        private IFixedTickable? FixedTickable { get; set; }

        [Inject]
        private ILateTickable? LateTickable { get; set; }

        [Inject]
        private IDisposable? Disposable { get; set; }

        [Inject]
        private ITerminatable? Terminatable { get; set; }

#if UNITY_2022_1_OR_NEWER
        [Inject]
        private IAsyncDisposable? AsyncDisposable { get; set; }
#endif

        [Inject]
        private IUnityKernelLogger? Logger { get; set; }

        private ConcurrentDictionary<IAsyncTickable, Task> activeAsyncTickables = new();

        private async void Start()
        {
            try
            {
                Initializable?.Initialize();

                if (AsyncInitializable is not null)
                {
                    await AsyncInitializable.InitializeAsync();
                }
            }
            catch (Exception exception)
            {
                LogException(exception);
            }
        }

        private async void Update()
        {
            try
            {
                Tickable?.Tick();
            }
            catch (Exception exception)
            {
                LogException(exception);
            }

            if (AsyncTickables is null)
            {
                return;
            }

            foreach (IAsyncTickable asyncTickable in AsyncTickables)
            {
                if (activeAsyncTickables.ContainsKey(asyncTickable))
                {
                    continue;
                }

                Task task = asyncTickable.TickAsync().AsTask();

                activeAsyncTickables.TryAdd(asyncTickable, task);

                _ = task.ContinueWith(result =>
                {
                    try
                    {
                        if (result.IsFaulted)
                        {
                            LogException(result.Exception);
                        }
                    }
                    finally
                    {
                        activeAsyncTickables.TryRemove(asyncTickable, out _);
                    }
                });
            }
        }

        private void FixedUpdate()
        {
            try
            {
                FixedTickable?.FixedTick();
            }
            catch (Exception exception)
            {
                LogException(exception);
            }
        }

        private void LateUpdate()
        {
            try
            {
                LateTickable?.LateTick();
            }
            catch (Exception exception)
            {
                LogException(exception);
            }
        }

#if UNITY_2022_1_OR_NEWER
        private async void OnDestroy()
#else
        private void OnDestroy()
#endif
        {
            try
            {
                Disposable?.Dispose();

#if UNITY_2022_1_OR_NEWER
                if (AsyncDisposable is not null)
                {
                    await AsyncDisposable.DisposeAsync();
                }
#endif
            }
            catch (Exception exception)
            {
                LogException(exception);
            }
        }

        private void OnApplicationQuit()
        {
            try
            {
                Terminatable?.Terminate();
            }
            catch (Exception exception)
            {
                LogException(exception);
            }
        }

        private void LogException(Exception exception)
        {
            if (Logger is not null)
            {
                Logger.LogException(exception);
                return;
            }

            // Should not happen, but better safe than sorry.
            Debug.LogWarning(
                "Exception encountered, but no logger is configured for the UnityKernel. You can (should) fix this " +
                "by registering a IUnityKernelLogger in one of your modules. To avoid silencing errors, directly " +
                "logging exception to the Unity console."
            );

            Debug.LogException(exception);
        }
    }
}