using System.Collections.Generic;
using CycloneGames.AssetManagement.Runtime;
using Cysharp.Threading.Tasks;
using GASSample.Misc;
using UnityEngine;
using UnityEngine.EventSystems;

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

                GameObject instance = Instantiate(prefab.AssetObject) as GameObject;

                // The whole point of this component: these objects must survive the scene that spawned
                // them, so the first transition does not take the camera or the UI with it.
                DontDestroyOnLoad(instance);

                // The debug console prefab ships its own EventSystem and Unity only allows one in the
                // project. Deactivate it - destroying it would leave the console's scripts with dead
                // references - and do it before the project's own EventSystem is spawned, otherwise
                // both are active for a frame and Unity logs a duplicate warning.
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
