using System.Collections;
using System.Threading.Tasks;

namespace MrWatts.Internal.FuelInject.Testing
{
    public sealed class SceneSetupResult
    {
        /// <summary>
        /// Enumerable that finishes when the scene is done loading.
        /// </summary>
        public IEnumerator SceneLoadingOperation { get; private set; }

        /// <summary>
        /// Task that resolves when all IInitializables are done.
        /// </summary>
        public Task InitializableTask { get; private set; }

        /// <summary>
        /// Task that resolves when all IAsyncInitializables are done.
        /// </summary>
        public Task AsyncInitializableTask { get; private set; }

        public SceneSetupResult(IEnumerator sceneLoadingOperation, Task initializableTask, Task asyncInitializableTask)
        {
            SceneLoadingOperation = sceneLoadingOperation;
            InitializableTask = initializableTask;
            AsyncInitializableTask = asyncInitializableTask;
        }
    }
}