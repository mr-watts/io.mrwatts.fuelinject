using UnityEngine;

namespace MrWatts.Internal.FuelInject.TestProject
{
    public sealed class MonoBehaviourWithBarDependency : MonoBehaviour
    {
        [Inject]
        private IBar Bar { get; set; } = default!;

        public IBar BarGetter { get => Bar; }
    }
}