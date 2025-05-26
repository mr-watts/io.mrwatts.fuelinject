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
            AsyncOperation operation = SceneManager.UnloadSceneAsync(scene);

            while (!operation.isDone)
            {
                yield return null;
            }
        }

        public IEnumerator UnloadAll()
        {
            // This loads in a dummy scene (additively) because Unity does not allow unloading the last scene. This way
            // we can still use UnloadSceneAsync to unload the scene completely.
            SceneManager.CreateScene($"FuelInjectTestDummy-{Guid.NewGuid()}");

            for (int i = 0; i < SceneManager.sceneCount; ++i)
            {
                Scene activeScene = SceneManager.GetSceneAt(i);

                // Unity spawns its own scene that we don't need to try to unload.
                if (activeScene.name.StartsWith("FuelInjectTestDummy", StringComparison.Ordinal) ||
                    activeScene.name.StartsWith("InitTestScene", StringComparison.Ordinal))
                {
                    continue;
                }

                yield return Unload(activeScene);
            }

            /*
                When all scenes get unloaded for the purposes of testing, we also want DontDestroyOnLoad objects to
                disappear. Ordinarily they should persist scene changes but during tests they shouldn't as tests should
                be isolated from one another and each scene test should start with a fully clean slate.
            */
            Array.ForEach(GetDontDestroyOnLoadGameObjects(), UnityEngine.Object.DestroyImmediate);
        }

        /// <summary>
        /// Retrieves all GameObjects that are DontDestroyOnLoad, which is shown as a separate scene, but isn't an
        /// actual scene that is returned in the list of scenes nor are these objects retrievable by ordinary means.
        /// </summary>
        /// <remarks>
        /// See also https://discussions.unity.com/t/editor-script-how-to-access-objects-under-dontdestroyonload-while-in-play-mode/646469/7
        /// </remarks>
        private static GameObject[] GetDontDestroyOnLoadGameObjects()
        {
            GameObject? dummy = new();
            UnityEngine.Object.DontDestroyOnLoad(dummy);

            Scene dontDestroyOnLoadScene = dummy.scene;

            UnityEngine.Object.DestroyImmediate(dummy);

            return dontDestroyOnLoadScene.GetRootGameObjects();
        }
    }
}