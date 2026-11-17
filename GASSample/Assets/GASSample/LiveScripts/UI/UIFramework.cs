using UnityEngine;

namespace GASSample.UI
{
    /// <summary>
    /// Marks the UIFramework prefab root as persistent. The prefab is loaded by
    /// <see cref="GASSample.AssetManagement.AssetResolverForDontDestroy"/> and must survive the
    /// scene changes that follow.
    /// </summary>
    public class UIFramework : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
