using System;
using Autofac;

namespace MrWatts.Internal.FuelInject
{
    /// <summary>
    /// Module creating service bindings necessary for the UnityKernel.
    /// </summary>
    public sealed class KernelModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterComposite<CompositeInitializable, IInitializable>().SingleInstance();
            builder.RegisterComposite<CompositeAsyncInitializable, IAsyncInitializable>().SingleInstance();
            builder.RegisterComposite<CompositeTickable, ITickable>().SingleInstance();
            builder.RegisterComposite<CompositeFixedTickable, IFixedTickable>().SingleInstance();
            builder.RegisterComposite<CompositeLateTickable, ILateTickable>().SingleInstance();
            builder.RegisterComposite<CompositeDisposable, IDisposable>().SingleInstance();
            builder.RegisterComposite<CompositeTerminatable, ITerminatable>().SingleInstance();
            // builder.RegisterComposite<CompositeAsyncDisposable, IAsyncDisposable>();
        }
    }
}