using Autofac;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MrWatts.Internal.DependencyInjection
{
    public sealed class UnitySceneDependencyInjector
    {
        private readonly UnityGameObjectDependencyInjector unityGameObjectDependencyInjector;

        public UnitySceneDependencyInjector(UnityGameObjectDependencyInjector unityGameObjectDependencyInjector)
        {
            this.unityGameObjectDependencyInjector = unityGameObjectDependencyInjector;
        }

        public void Inject(IContainer container)
        {
            foreach (GameObject gameObject in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                unityGameObjectDependencyInjector.Inject(gameObject, container);
            }
        }
    }
}