using System;

namespace MrWatts.Internal.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public sealed class InjectAttribute : Attribute
    {
    }
}