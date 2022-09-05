using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MrWatts.Internal.FuelInject.Testing
{
    public sealed class SceneLoader
    {
        public IEnumerator Load(string name)
        {
            AsyncOperation loader = SceneManager.LoadSceneAsync(name);

            while (!loader.isDone)
            {
                yield return null;
            }
        }
    }
}