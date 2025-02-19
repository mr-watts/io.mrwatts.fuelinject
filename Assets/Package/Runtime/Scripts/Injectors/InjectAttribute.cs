using System;

namespace MrWatts.Internal.FuelInject
{
#pragma warning disable RCS1251 Remove unnecessary braces (Roslynator reports this incorrectly in CI for an unknown reason)
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public sealed class InjectAttribute : Attribute
    {
    }
#pragma warning enable RCS1251
}