using Autofac;
using Autofac.Core;
using System.Linq;
using System.Reflection;

namespace MrWatts.Internal.FuelInject
{
    public sealed class ObjectDependencyInjector : IInjector<object>
    {
        public void Inject(object @object, IComponentContext context)
        {
            context.InjectProperties(@object, new DelegatePropertySelector((p, _) => p.GetCustomAttributes<InjectAttribute>().Any()));
        }
    }
}