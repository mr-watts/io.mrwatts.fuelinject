using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MrWatts.Internal.FuelInject.Testing
{
    public sealed class SceneLoader
    {
        public IEnumerator Load(string name, bool isAdditive = false)
        {
            UnityEngine.Debug.Log($"Loading scene {name} {isAdditive}");

            AsyncOperation operation = SceneManager.LoadSceneAsync(name, isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);

            while (!operation.isDone)
            {
                yield return null;
            }

            UnityEngine.Debug.Log($"Checking scenes after load");

            for (int i = 0; i < SceneManager.sceneCount; ++i)
            {
                UnityEngine.Debug.Log($"   - {SceneManager.GetSceneAt(i).name} is now loaded");
            }
        }
    }
}