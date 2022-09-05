using System;
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
        private IFixedTickable? FixedTickable { get; set; }

        [Inject]
        private ILateTickable? LateTickable { get; set; }

        [Inject]
        private IDisposable? Disposable { get; set; }

        // [Inject]
        // private IAsyncDisposable AsyncDisposable { get; set; }

        private async void Start()
        {
            Initializable?.Initialize();

            if (AsyncInitializable is not null)
            {
                await AsyncInitializable.InitializeAsync();
            }
        }

        private void Update()
        {
            Tickable?.Tick();
        }

        private void FixedUpdate()
        {
            FixedTickable?.FixedTick();
        }

        private void LateUpdate()
        {
            LateTickable?.LateTick();
        }

        private /*async*/ void OnDestroy()
        {
            Disposable?.Dispose();

            // if (AsyncDisposable is not null)
            // {
            //     await AsyncDisposable.DisposeAsync();
            // }
        }
    }
}