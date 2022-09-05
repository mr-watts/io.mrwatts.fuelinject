namespace MrWatts.Internal.FuelInject
{
    /// <summary>
    /// <para>Interface for classes that exposes priorities.</para>
    /// <para>This can be used for modules to indicate that they wish to load before or other modules.</para>
    /// </summary>
    public interface IPrioritizable
    {
        int Priority { get; }
    }
}