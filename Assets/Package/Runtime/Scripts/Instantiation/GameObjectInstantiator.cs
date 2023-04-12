using UnityEngine;

namespace MrWatts.Internal.FuelInject
{
    public sealed class GameObjectInstantiator : IGameObjectInstantiator
    {
        private readonly IInjector<GameObject> injector;

        public GameObjectInstantiator(IInjector<GameObject> injector)
        {
            this.injector = injector;
        }

        public GameObject Instantiate()
        {
            return new GameObject();
        }

        public GameObject Instantiate(GameObject original)
        {
            bool shouldBeActive = PreprocessOriginal(original);

            GameObject newObject = GameObject.Instantiate(original);

            PostprocessOriginal(original, shouldBeActive);
            PostprocessNewObject(newObject, shouldBeActive);

            return newObject;
        }

        public GameObject Instantiate(GameObject original, Transform parent)
        {
            bool shouldBeActive = PreprocessOriginal(original);

            GameObject newObject = GameObject.Instantiate(original, parent);

            PostprocessOriginal(original, shouldBeActive);
            PostprocessNewObject(newObject, shouldBeActive);

            return newObject;
        }

        public GameObject Instantiate(GameObject original, Transform parent, bool instantiateInWorldSpace)
        {
            bool shouldBeActive = PreprocessOriginal(original);

            GameObject newObject = GameObject.Instantiate(original, parent, instantiateInWorldSpace);

            PostprocessOriginal(original, shouldBeActive);
            PostprocessNewObject(newObject, shouldBeActive);

            return newObject;
        }

        public GameObject Instantiate(GameObject original, Vector3 position, Quaternion rotation)
        {
            bool shouldBeActive = PreprocessOriginal(original);

            GameObject newObject = GameObject.Instantiate(original, position, rotation);

            PostprocessOriginal(original, shouldBeActive);
            PostprocessNewObject(newObject, shouldBeActive);

            return newObject;
        }

        public GameObject Instantiate(GameObject original, Vector3 position, Quaternion rotation, Transform parent)
        {
            bool shouldBeActive = PreprocessOriginal(original);

            GameObject newObject = GameObject.Instantiate(original, position, rotation, parent);

            PostprocessOriginal(original, shouldBeActive);
            PostprocessNewObject(newObject, shouldBeActive);

            return newObject;
        }

        private bool PreprocessOriginal(GameObject original)
        {
            bool wasActive = original.activeSelf;
            original.SetActive(false);

            return wasActive;
        }

        private void PostprocessOriginal(GameObject original, bool wasActiveBefore)
        {
            original.SetActive(wasActiveBefore);
        }

        private void PostprocessNewObject(GameObject newObject, bool shouldBeActive)
        {
            injector.Inject(newObject);

            newObject.SetActive(shouldBeActive);
        }
    }
}