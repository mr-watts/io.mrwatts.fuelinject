using UnityEngine;

namespace MrWatts.Internal.FuelInject
{
    public sealed class UnityGameObjectDependencyInjector : IInjector<GameObject>
    {
        private readonly IInjector<object> objectInjector;

        public UnityGameObjectDependencyInjector(IInjector<object> objectInjector)
        {
            this.objectInjector = objectInjector;
        }

        public void Inject(GameObject @object)
        {
            objectInjector.Inject(@object);

            foreach (Component component in @object.GetComponents<Component>())
            {
                if (component != null)
                {
                    objectInjector.Inject(component);
                }
            }

            for (int i = 0; i < @object.transform.childCount; ++i)
            {
                Inject(@object.transform.GetChild(i).gameObject);
            }
        }
    }
}