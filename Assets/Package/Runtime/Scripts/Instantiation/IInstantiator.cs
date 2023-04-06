namespace MrWatts.Internal.FuelInject
{
    public interface IInstantiator<T>
    {
        T Instantiate();
    }
}