using Autofac;
using Autofac.Core;
using System.Linq;
using System.Reflection;

namespace MrWatts.Internal.FuelInject
{
    public sealed class ObjectDependencyInjector
    {
        public void Inject(object @object, IContainer container)
        {
            container.InjectProperties(@object, new DelegatePropertySelector((p, _) => p.GetCustomAttributes<InjectAttribute>().Any()));
        }
    }
}