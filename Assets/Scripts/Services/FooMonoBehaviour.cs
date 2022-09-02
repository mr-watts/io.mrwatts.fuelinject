using UnityEngine;

namespace MrWatts.Internal.FuelInject.TestProject
{
    public sealed class FooMonoBehaviour : MonoBehaviour
    {
        [Inject]
        private Bar Bar { get; set; } = default!;

        [Inject]
        private IInjector<GameObject> Injector { get; set; } = default!;

        public void Start()
        {
            Debug.Log($"Bar is {Bar}");
            Debug.Log($"Injected injector is {Injector}");
        }
    }
}