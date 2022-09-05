using UnityEngine;

namespace MrWatts.Internal.FuelInject.Testing.Utility
{
    /// <summary>
    /// Utility class for finding GameObjects in the scene (for the purpose of writing tests).
    /// </summary>
    public static class GameObjectFinder
    {
        /// <summary>
        /// Same as Find with a type, but throws an exception if the object is not found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mustBeActive"></param>
        /// <exception cref="GameObjectFinderException">If the object was not found.</exception>
        public static T Get<T>(bool mustBeActive = false) where T : Object
        {
            T? gameObject = Find<T>(mustBeActive);

            if (gameObject is null)
            {
                throw new GameObjectFinderException($"No game object of type '{typeof(T).Name}' (mustBeActive set to {mustBeActive}) found whilst one was expected");
            }

            return gameObject;
        }

        /// <summary>
        /// Same as Find with a name, but throws an exception if the object is not found.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mustBeActive"></param>
        /// <exception cref="GameObjectFinderException">If the object was not found.</exception>
        public static GameObject Get(string name, bool mustBeActive = false)
        {
            GameObject? gameObject = Find(name, mustBeActive);

#pragma warning disable UNT0029
            if (gameObject is null)
            {
                throw new GameObjectFinderException($"No game object with name '{name}' (mustBeActive set to {mustBeActive}) found whilst one was expected");
            }
#pragma warning restore UNT0029

            return gameObject;
        }

        /// <summary>
        /// Works like Object.FindObjectOfType, but has a shorthand that searches for inactive items as well.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mustBeActive"></param>
        public static T? Find<T>(bool mustBeActive = false) where T : Object
        {
            return Object.FindObjectOfType<T>(!mustBeActive);
        }

        /// <summary>
        /// Works like GameObject.Find, but can also find items that are inactive.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mustBeActive"></param>
        public static GameObject? Find(string name, bool mustBeActive = false)
        {
            if (mustBeActive)
            {
                return GameObject.Find(name);
            }

            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (string.Equals(gameObject.name, name, System.StringComparison.Ordinal))
                {
                    return gameObject;
                }
            }

            return null;
        }
    }
}