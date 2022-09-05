using System.Threading.Tasks;
using UnityEngine;

namespace MrWatts.Internal.FuelInject.TestProject
{
    public sealed class AsyncInitializableMonoBehaviour : MonoBehaviour, IAsyncInitializable
    {
        public bool IsInitialized { get; private set; }

        public ValueTask InitializeAsync()
        {
            IsInitialized = true;

            return new ValueTask(Task.CompletedTask);
        }
    }
}