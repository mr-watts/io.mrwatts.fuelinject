using System.Collections;

namespace MrWatts.Internal.FuelInject.Testing
{
    public sealed class SceneSetupResult
    {
        /// <summary>
        /// Enumerable that finishes when the scene is done loading.
        /// </summary>
        public IEnumerator SceneLoadingOperation { get; private set; }

        public SceneSetupResult(IEnumerator sceneLoadingOperation)
        {
            SceneLoadingOperation = sceneLoadingOperation;
        }
    }
}