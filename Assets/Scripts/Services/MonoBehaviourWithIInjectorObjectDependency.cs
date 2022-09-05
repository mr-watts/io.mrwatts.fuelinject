using UnityEngine;

namespace MrWatts.Internal.FuelInject.TestProject
{
    public sealed class MonoBehaviourWithIInjectorObjectDependency : MonoBehaviour
    {
        [Inject]
        private IInjector<object> Injector { get; set; } = default!;

        public IInjector<object> InjectorGetter { get => Injector; }
    }
}