using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MrWatts.Internal.FuelInject.Testing
{
    public sealed class SceneLoader
    {
        public IEnumerator Load(string name, bool isAdditive = false)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(name, isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);

            while (!operation.isDone)
            {
                yield return null;
            }
        }
    }
}