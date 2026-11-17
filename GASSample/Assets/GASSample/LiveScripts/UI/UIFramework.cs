using UnityEngine;

namespace GASSample.UI
{
    /// <summary>
    /// Marks the UIFramework prefab root as persistent. Spawned by
    /// <see cref="GASSample.AssetManagement.AssetResolverForDontDestroy"/>.
    /// </summary>
    public class UIFramework : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
