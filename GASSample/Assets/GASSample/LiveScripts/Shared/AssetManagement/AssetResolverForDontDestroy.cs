using System.Collections.Generic;
using Addler.Runtime.Core.LifetimeBinding;
using CycloneGames.Logger;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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
        [SerializeField]
        private List<AddressableResolverData> dontDestroyAddressablePaths = new List<AddressableResolverData>();

        // Consider making this public or initializing if always a singleton
        [SerializeField]
        private bool isSingleton = true; // Default to true if usually a singleton

        public static AssetResolverForDontDestroy Instance { get; private set; }

        void Awake()
        {
            if (isSingleton)
            {
                if (Instance != null && Instance != this)
                {
                    CLogger.LogWarning($"Duplicate instance of {nameof(AssetResolverForDontDestroy)} found. Destroying this one.");
                    Destroy(gameObject);
                    return;
                }

                Instance = this;
                DontDestroyOnLoad(gameObject); // Make this manager persistent
            }

            foreach (AddressableResolverData pathData in dontDestroyAddressablePaths)
            {
                Addressables.LoadAssetAsync<GameObject>(pathData.AddressablePath).BindTo(this.gameObject)
                    .Completed += (handle) =>
                    {
                        if (handle.Status == AsyncOperationStatus.Succeeded)
                        {
                            if (handle.Result == null)
                            {
                                CLogger.LogError($"Addressable path '{pathData}' loaded successfully but the result was null.");
                                return;
                            }
                            // Instantiate the loaded prefab. Optionally parent it to this resolver.
                            GameObject instance = Instantiate(handle.Result /*, transform */); // Uncomment ', transform' to parent

                            // Make the instantiated GameObject persistent
                            DontDestroyOnLoad(instance);
                            // Debug.Log($"Successfully loaded and instantiated '{path}' as DontDestroyOnLoad.");
                        }
                        else
                        {
                            CLogger.LogError($"Failed to load Addressable: {pathData}. Error: {handle.OperationException}");
                        }
                    };
            }
        }
    }

    [System.Serializable]
    public class AddressableResolverData
    {
        public string DisplayName;
        public string AddressablePath;
    }
}