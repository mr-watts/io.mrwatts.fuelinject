using System;

namespace MrWatts.Internal.FuelInject
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public sealed class InjectAttribute : Attribute
    {
    }
}