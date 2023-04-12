using UnityEngine;

namespace MrWatts.Internal.FuelInject
{
    public interface IGameObjectInstantiator : IInstantiator<GameObject>
    {
        GameObject Instantiate(GameObject original);
        GameObject Instantiate(GameObject original, Transform parent);
        GameObject Instantiate(GameObject original, Transform parent, bool instantiateInWorldSpace);
        GameObject Instantiate(GameObject original, Vector3 position, Quaternion rotation);
        GameObject Instantiate(GameObject original, Vector3 position, Quaternion rotation, Transform parent);
    }
}