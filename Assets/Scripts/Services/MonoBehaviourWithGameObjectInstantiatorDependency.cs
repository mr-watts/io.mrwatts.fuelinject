using UnityEngine;

namespace MrWatts.Internal.FuelInject.TestProject
{
    public sealed class MonoBehaviourWithGameObjectInstantiatorDependency : MonoBehaviour
    {
        [Inject]
        public IInstantiator<GameObject> GenericInstantiator { get; private set; } = default!;

        [Inject]
        public IGameObjectInstantiator SpecificInstantiator { get; private set; } = default!;
    }
}