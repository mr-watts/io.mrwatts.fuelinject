using UnityEngine;
using UnityEngine.SceneManagement;

namespace MrWatts.Internal.FuelInject.TestProject
{
    public sealed class MonoBehaviourWithIInjectorSceneDependency : MonoBehaviour
    {
        [Inject]
        private IInjector<Scene> Injector { get; set; } = default!;

        public IInjector<Scene> InjectorGetter { get => Injector; }
    }
}