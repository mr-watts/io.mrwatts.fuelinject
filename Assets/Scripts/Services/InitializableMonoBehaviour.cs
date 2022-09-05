using UnityEngine;

namespace MrWatts.Internal.FuelInject.TestProject
{
    public sealed class InitializableMonoBehaviour : MonoBehaviour, IInitializable
    {
        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            IsInitialized = true;
        }
    }
}