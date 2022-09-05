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
        private IInitializable Initializable { get; set; } = default!;

        [Inject]
        private IAsyncInitializable AsyncInitializable { get; set; } = default!;

        [Inject]
        private ITickable Tickable { get; set; } = default!;

        [Inject]
        private IFixedTickable FixedTickable { get; set; } = default!;

        [Inject]
        private ILateTickable LateTickable { get; set; } = default!;

        [Inject]
        private IDisposable Disposable { get; set; } = default!;

        // [Inject]
        // private IAsyncDisposable AsyncDisposable { get; set; } = default!;

        private async void Start()
        {
            Initializable.Initialize();

            await AsyncInitializable.InitializeAsync();
        }

        private void Update()
        {
            Tickable.Tick();
        }

        private void FixedUpdate()
        {
            FixedTickable.FixedTick();
        }

        private void LateUpdate()
        {
            LateTickable.LateTick();
        }

        private /*async*/ void OnDestroy()
        {
            Disposable.Dispose();

            // await AsyncDisposable.DisposeAsync();
        }
    }
}