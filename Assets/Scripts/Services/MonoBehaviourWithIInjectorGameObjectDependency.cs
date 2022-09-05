using UnityEngine;

namespace MrWatts.Internal.FuelInject.TestProject
{
    public sealed class MonoBehaviourWithIInjectorGameObjectDependency : MonoBehaviour
    {
        [Inject]
        private IInjector<GameObject> Injector { get; set; } = default!;

        public IInjector<GameObject> InjectorGetter { get => Injector; }
    }
}