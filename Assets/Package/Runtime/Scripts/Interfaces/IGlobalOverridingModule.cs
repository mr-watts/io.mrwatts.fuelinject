namespace MrWatts.Internal.FuelInject
{
    /// <summary>
    /// Module that is intended to override registrations in the container by non-conventional means.
    /// </summary>
    /// <remarks>
    /// This class is not intended for ordinary or daily use. If you need to override registrations, you can simply add
    /// another module to your loader that is loaded after the original ones through IPrioritizable or by ordering it in
    /// the startup modules. This interface is instead meant to provide a global, non-disablable means of adding
    /// overrides to (all) containers in the scene mainly for purposes such as executing tests.
    /// </remarks>
    public interface IGlobalOverridingModule
    {
    }
}