using System.Collections.Generic;
using CycloneGames.AssetManagement.Runtime;
using CycloneGames.Logger;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GASSample.AssetManagement
{
    /// <summary>
    /// Manages the loading and persistence of Addressable GameObjects that are intended to survive scene transitions
    /// via <see cref="UnityEngine.Object.DontDestroyOnLoad"/>. This class helps prevent issues such as lost references
    /// or premature unloading of Addressable assets that are marked for persistence across scenes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Problem Addressed:
    /// When GameObjects are loaded via the Addressables system and then marked with <see cref="UnityEngine.Object.DontDestroyOnLoad"/>,
    /// issues can arise during scene transitions or when the Addressable assets' containing groups are managed.
    /// Specifically, references to these objects might be lost, or the objects themselves might be inadvertently destroyed
    /// if not handled correctly, particularly if they originate from Addressable scenes that are subsequently unloaded.
    /// </para>
    /// <para>
    /// Solution Provided by This Class:
    /// This class acts as a centralized resolver and manager for such persistent Addressable GameObjects.
    /// It ensures that:
    /// <list type="bullet">
    ///   <item><description>Specified GameObjects are loaded from their Addressable paths.</description></item>
    ///   <item><description>They are instantiated into the scene.</description></item>
    ///   <item><description>The instantiated GameObjects are correctly marked with <see cref="UnityEngine.Object.DontDestroyOnLoad"/>.</description></item>
    ///   <item><description>The Addressable load operations are bound to the lifetime of this resolver (if it's persistent),
    ///   ensuring that the underlying Addressable assets (handles) are appropriately managed and not prematurely released
    ///   as long as these objects are intended to persist.</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Recommended Best Practice:
    /// GameObjects that need to be persistent across scenes using <see cref="UnityEngine.Object.DontDestroyOnLoad"/>
    /// and are managed by the Addressables system should ideally not be placed directly within Addressable scenes
    /// that are loaded and unloaded additively. Instead, they should be instantiated and managed by a dedicated,
    /// persistent service like this <see cref="AssetResolverForDontDestroy"/> class. This approach ensures
    /// that the creation of these objects and the management of their Addressable handles are controlled independently
    /// of any individual scene's lifecycle.
    /// </para>
    /// </remarks>
    public class AssetResolverForDontDestroy : MonoBehaviour
    {
        private const string DEBUG_FLAG = "[AssetResolver]";

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
                CLogger.LogInfo($"{DEBUG_FLAG} Instantiate: {prefab.AssetObject.name}");
            }
        }
    }

    [System.Serializable]
    public class AssetResolverData
    {
        public string DisplayName;
        public string AddressablePath;
    }
}