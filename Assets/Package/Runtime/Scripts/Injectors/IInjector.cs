using Autofac;

namespace MrWatts.Internal.FuelInject
{
    public interface IInjector<T>
    {
        void Inject(T @object, IComponentContext context);
    }
}