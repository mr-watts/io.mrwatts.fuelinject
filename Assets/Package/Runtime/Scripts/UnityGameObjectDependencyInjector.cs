using Autofac;
using UnityEngine;

namespace MrWatts.Internal.DependencyInjection
{
    public sealed class UnityGameObjectDependencyInjector
    {
        private readonly ObjectDependencyInjector objectDependencyInjector;

        public UnityGameObjectDependencyInjector(ObjectDependencyInjector objectDependencyInjector)
        {
            this.objectDependencyInjector = objectDependencyInjector;
        }

        public void Inject(GameObject @object, IContainer container)
        {
            objectDependencyInjector.Inject(@object, container);

            foreach (var component in @object.GetComponents<Component>())
            {
                if (component is not null)
                {
                    objectDependencyInjector.Inject(component, container);
                }
            }

            for (int i = 0; i < @object.transform.childCount; ++i)
            {
                Inject(@object.transform.GetChild(i).gameObject, container);
            }
        }
    }
}