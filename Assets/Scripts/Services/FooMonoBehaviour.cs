using UnityEngine;

namespace MrWatts.Internal.FuelInject.TestProject
{
    public sealed class FooMonoBehaviour : MonoBehaviour
    {
        [Inject]
        private Bar Bar { get; set; } = default!;

        public Bar BarGetter { get => Bar; }
    }
}