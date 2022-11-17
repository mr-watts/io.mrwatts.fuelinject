using Autofac;
using Autofac.Core;
using System.Linq;
using System.Reflection;

namespace MrWatts.Internal.FuelInject
{
    public sealed class ObjectDependencyInjector : IInjector<object>
    {
        private readonly IComponentContext context;

        public ObjectDependencyInjector(IComponentContext context)
        {
            this.context = context;
        }

        public void Inject(object @object)
        {
            context.InjectProperties(@object, new DelegatePropertySelector((p, _) => p.GetCustomAttributes<InjectAttribute>().Any()));
        }
    }
}