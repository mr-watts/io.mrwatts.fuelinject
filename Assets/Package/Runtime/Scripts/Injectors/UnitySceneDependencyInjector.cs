using Autofac;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MrWatts.Internal.FuelInject
{
    public sealed class UnitySceneDependencyInjector : IInjector<Scene>
    {
        private readonly IInjector<GameObject> gameObjectInjector;

        public UnitySceneDependencyInjector(IInjector<GameObject> gameObjectInjector)
        {
            this.gameObjectInjector = gameObjectInjector;
        }

        public void Inject(Scene @object, IComponentContext context)
        {
            foreach (GameObject gameObject in @object.GetRootGameObjects())
            {
                gameObjectInjector.Inject(gameObject, context);
            }
        }
    }
}