using System;
using Autofac;
using Autofac.Core;

namespace MrWatts.Internal.FuelInject
{
    /// <summary>
    /// Proxies IComponentContext calls to a configurable delegate.
    /// </summary>
    public sealed class ConfigurableComponentContextProxy : IComponentContext
    {
        public IComponentContext? Delegatee { get; set; }

        private IComponentContext ValidDelegatee
        {
            get
            {
                if (Delegatee is null)
                {
                    throw new Exception("Component resolution was requested, but can't proxy call since no delegatee was configured yet");
                }

                return Delegatee;
            }
        }

        public IComponentRegistry ComponentRegistry => ValidDelegatee.ComponentRegistry;

        public object ResolveComponent(ResolveRequest request)
        {
            return ValidDelegatee.ResolveComponent(request);
        }
    }
}