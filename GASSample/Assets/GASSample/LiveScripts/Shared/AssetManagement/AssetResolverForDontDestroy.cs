using System.Collections.Generic;
using CycloneGames.AssetManagement.Runtime;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GASSample.AssetManagement
{
    /// <summary>
    /// Spawns the prefabs that must outlive a scene change (debug tools, event system, main camera,
    /// UIFramework). The addressable handles stay alive for the application lifetime, which is what
    /// keeps those objects valid after the scene that loaded them is unloaded.
    /// </summary>
    public class AssetResolverForDontDestroy : MonoBehaviour
    {
        [SerializeField]
        private List<AssetResolverData> dontDestroyAddressablePaths = new List<AssetResolverData>();

        public async UniTask InitializeAsync(IAssetModule assetModule)
        {
            var pkg = assetModule.GetPackage(AssetPackageName.DefaultPackage);

            foreach (AssetResolverData pathData in dontDestroyAddressablePaths)
            {
                var prefab = pkg.LoadAssetAsync<GameObject>(pathData.AddressablePath);
                await prefab.Task;
                await InstantiateAsync(prefab.AssetObject);
            }
        }
    }

    [System.Serializable]
    public class AssetResolverData
    {
        public string AddressablePath;
    }
}
