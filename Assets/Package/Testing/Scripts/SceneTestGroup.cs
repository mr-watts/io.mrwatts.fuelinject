using System.Collections;
using UnityEngine.TestTools;

namespace MrWatts.Internal.FuelInject.Testing
{
    /// <summary>
    /// Represents a group of scene tests. Extend this class to make writing scene tests more convenient.
    /// </summary>
    public abstract class SceneTestGroup
    {
        /// <summary>
        /// <para>The name of the scene to load for the test.</para>
        /// <para>Set to or leave this at null if you want to handle this yourself.</para>
        /// </summary>
        protected virtual string? SceneName { get; }

        protected SceneLoader SceneLoader { get; } = new();
        protected SceneUnloader SceneUnloader { get; } = new();

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            if (SceneName is not null)
            {
                yield return SceneLoader.Load(SceneName);
            }
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return SceneUnloader.UnloadAll();
        }
    }
}