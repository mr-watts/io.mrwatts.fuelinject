using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MrWatts.Internal.FuelInject.Testing
{
    public sealed class SceneUnloader
    {
        public IEnumerator Unload(Scene scene)
        {
            AsyncOperation loader = SceneManager.UnloadSceneAsync(scene);

            while (!loader.isDone)
            {
                yield return null;
            }
        }

        public IEnumerator UnloadAll()
        {
            // This loads in a dummy scene (additively) because Unity does not allow unloading the last scene. This way
            // we can still use UnloadSceneAsync to unload the scene completely.
            SceneManager.CreateScene($"Dummy-{Guid.NewGuid()}");

            for (int i = 0; i < SceneManager.sceneCount; ++i)
            {
                Scene activeScene = SceneManager.GetSceneAt(i);

                // Unity spawns its own scene that we don't need to try to unload.
                if (activeScene.name.StartsWith("DummyScene", StringComparison.Ordinal) ||
                    activeScene.name.StartsWith("InitTestScene", StringComparison.Ordinal))
                {
                    continue;
                }

                yield return Unload(activeScene);
            }
        }
    }
}