using UnityEngine;

namespace MrWatts.Internal.FuelInject.TestProject
{
    public sealed class MonoBehaviourWithBarDependency : MonoBehaviour
    {
        [Inject]
        private Bar Bar { get; set; } = default!;

        public Bar BarGetter { get => Bar; }
    }
}