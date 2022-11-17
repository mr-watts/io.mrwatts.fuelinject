using Autofac;
using Autofac.Diagnostics;

namespace MrWatts.Internal.FuelInject
{
    /// <summary>
    /// <para>Module that activates container diagnostics.</para>
    /// <para>See also <see href="https://autofac.readthedocs.io/en/latest/advanced/debugging.html#quick-start"/>.</para>
    /// </summary>
    public sealed class ContainerDiagnosticsModule : Module
    {
        private readonly DiagnosticTracerBase tracer;

        public ContainerDiagnosticsModule(DiagnosticTracerBase tracer)
        {
            this.tracer = tracer;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterBuildCallback(context => (context as IContainer)!.SubscribeToDiagnostics(tracer));
        }
    }
}