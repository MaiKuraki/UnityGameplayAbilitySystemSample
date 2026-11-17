using System.Collections.Generic;
using CycloneGames.AssetManagement.Runtime;
using Cysharp.Threading.Tasks;
using GASSample.Misc;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GASSample.AssetManagement
{
    /// <summary>
    /// Spawns the prefabs that must outlive a scene change and keeps them alive with the package
    /// handles.
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

                GameObject instance = Instantiate(prefab.AssetObject) as GameObject;

                DontDestroyOnLoad(instance);

                // The debug console ships its own EventSystem and Unity allows one. Deactivate it
                // before the project's EventSystem spawns, or both are active for a frame.
                if (instance.GetComponent<GASSampleEventSystem>() == null)
                {
                    foreach (EventSystem eventSystem in instance.GetComponentsInChildren<EventSystem>(true))
                    {
                        eventSystem.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class AssetResolverData
    {
        public string AddressablePath;
    }
}
