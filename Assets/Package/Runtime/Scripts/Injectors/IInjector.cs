using Autofac;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MrWatts.Internal.FuelInject
{
    public interface IInjector<T>
    {
        public void Inject(T @object, IContainer container);
    }
}