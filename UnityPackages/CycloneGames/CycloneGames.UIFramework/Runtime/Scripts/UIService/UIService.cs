using System;
using CycloneGames.Factory.Runtime; // For IUnityObjectSpawner
using CycloneGames.Service; // For IMainCameraService
using CycloneGames.AssetManagement; // For IAssetPathBuilderFactory
using CycloneGames.AssetManagement.Integrations.Common;

namespace CycloneGames.UIFramework
{
    public interface IUIService
    {
        /// <summary>
        /// Opens a UI by its registered name.
        /// </summary>
        /// <param name="windowName">The name of the UI window to open.</param>
        /// <param name="onWindowCreated">Optional callback invoked when the window is created.</param>
        void OpenUI(string windowName, System.Action<UIWindow> onWindowCreated = null);

        /// <summary>
        /// Closes a UI by its registered name.
        /// </summary>
        /// <param name="windowName">The name of the UI window to close.</param>
        void CloseUI(string windowName);

        /// <summary>
        /// Checks if a UI window is currently considered valid (e.g., open and active).
        /// </summary>
        /// <param name="windowName">The name of the UI window.</param>
        /// <returns>True if the window is valid, false otherwise.</returns>
        bool IsUIWindowValid(string windowName);

        /// <summary>
        /// Gets a reference to an open UI window by its name.
        /// </summary>
        /// <param name="windowName">The name of the UI window.</param>
        /// <returns>The UIWindow instance if found and active, otherwise null.</returns>
        UIWindow GetUIWindow(string windowName); // Renamed from GetUIPage for consistency

        // Optional: Methods to manage UI camera stacking if not handled internally by UIManager
        // void AddUICameraToMainCameraStack();
        // void RemoveUICameraFromMainCameraStack();

        (float, float) GetRootCanvasSize();

        void Initialize(IAssetPathBuilderFactory factory, IUnityObjectSpawner spawner, IMainCameraService cameraService);
        void Initialize(IAssetPathBuilderFactory factory, IUnityObjectSpawner spawner, IMainCameraService cameraService, IAssetPackage package);
    }

    public class UIService : IDisposable, IUIService
    {
        private const string DEBUG_FLAG = "[UIService]";
        private UIManager uiManagerInstance; // Renamed for clarity

        // Dependencies are injected via constructor
        private IAssetPathBuilderFactory assetPathBuilderFactory;
        private IUnityObjectSpawner objectSpawner;
        private IMainCameraService mainCameraService;

        private bool isInitialized = false;

        // Default constructor might be used if service locator pattern is used elsewhere to provide dependencies later.
        // However, constructor injection is generally preferred for clarity of dependencies.
        public UIService()
        {
            // This constructor implies dependencies will be set via properties or an Init method,
            // or that a parameterless constructor is needed for some DI frameworks.
            // For this example, assuming the parameterized constructor is primary.
            UnityEngine.Debug.LogWarning($"{DEBUG_FLAG} UIService created with default constructor. Ensure Initialize or parameterized constructor is used.");
        }

        public UIService(IAssetPathBuilderFactory factory, IUnityObjectSpawner spawner, IMainCameraService cameraService)
        {
            Initialize(factory, spawner, cameraService);
        }

        public UIService(IAssetPathBuilderFactory factory, IUnityObjectSpawner spawner, IMainCameraService cameraService, IAssetPackage package)
        {
            Initialize(factory, spawner, cameraService, package);
        }

        public void Initialize(IAssetPathBuilderFactory factory, IUnityObjectSpawner spawner, IMainCameraService cameraService)
        {
            if (isInitialized)
            {
                UnityEngine.Debug.LogWarning($"{DEBUG_FLAG} UIService already initialized. Operation aborted.");
                return;
            }
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            if (spawner == null) throw new ArgumentNullException(nameof(spawner));
            // cameraService can be optional depending on requirements
            // if (cameraService == null) throw new ArgumentNullException(nameof(cameraService));

            this.assetPathBuilderFactory = factory;
            this.objectSpawner = spawner;
            this.mainCameraService = cameraService;

            InitializeUIManager(null);
            isInitialized = true;
        }

        public void Initialize(IAssetPathBuilderFactory factory, IUnityObjectSpawner spawner, IMainCameraService cameraService, IAssetPackage package)
        {
            if (isInitialized)
            {
                UnityEngine.Debug.LogWarning($"{DEBUG_FLAG} UIService already initialized. Operation aborted.");
                return;
            }
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            if (spawner == null) throw new ArgumentNullException(nameof(spawner));

            this.assetPathBuilderFactory = factory;
            this.objectSpawner = spawner;
            this.mainCameraService = cameraService;

            InitializeUIManager(package);
            isInitialized = true;
        }

        // This method could also be an explicit Init if dependencies aren't constructor-injected.
        private void InitializeUIManager(IAssetPackage package)
        {
            // Try to find an existing UIManager in the scene.
            uiManagerInstance = UnityEngine.GameObject.FindFirstObjectByType<UIManager>();

            if (uiManagerInstance == null)
            {
                // If not found, create one. This UIManager GameObject should persist.
                UnityEngine.GameObject managerObject = new UnityEngine.GameObject("UIManager_RuntimeInstance");
                uiManagerInstance = managerObject.AddComponent<UIManager>();
                UnityEngine.Object.DontDestroyOnLoad(managerObject); // Make it persist across scene loads
                UnityEngine.Debug.Log($"{DEBUG_FLAG} UIManager instance created and marked DontDestroyOnLoad.");
            }
            else
            {
                UnityEngine.Debug.Log($"{DEBUG_FLAG} Found existing UIManager instance in the scene.");
            }

            // Initialize the UIManager instance with the provided dependencies.
            var pkg = package ?? AssetManagementLocator.DefaultPackage;
            uiManagerInstance.Initialize(assetPathBuilderFactory, objectSpawner, mainCameraService, pkg);
        }

        private bool CheckInitialization()
        {
            if (!isInitialized || uiManagerInstance == null)
            {
                UnityEngine.Debug.LogError($"{DEBUG_FLAG} UIService or UIManager is not initialized. Operation aborted.");
                return false;
            }
            return true;
        }

        public bool IsUIWindowValid(string windowName)
        {
            if (!CheckInitialization()) return false;
            return uiManagerInstance.IsUIWindowValid(windowName);
        }

        public void OpenUI(string windowName, Action<UIWindow> onWindowCreated = null)
        {
            if (!CheckInitialization())
            {
                onWindowCreated?.Invoke(null); // Notify failure
                return;
            }
            uiManagerInstance.OpenUI(windowName, onWindowCreated);
        }

        public void CloseUI(string windowName)
        {
            if (!CheckInitialization()) return;
            uiManagerInstance.CloseUI(windowName);
        }

        /// <summary>
        /// Optionally open and await until the window reports Opened (strict sequencing use-cases).
        /// </summary>
        public async Cysharp.Threading.Tasks.UniTask<UIWindow> OpenUIAndWait(string windowName)
        {
            if (!CheckInitialization()) return null;
            var tcs = new Cysharp.Threading.Tasks.UniTaskCompletionSource<UIWindow>();
            uiManagerInstance.OpenUI(windowName, w =>
            {
                if (w == null) tcs.TrySetResult(null);
                else tcs.TrySetResult(w);
            });
            return await tcs.Task;
        }

        public UIWindow GetUIWindow(string windowName)
        {
            if (!CheckInitialization()) return null;
            return uiManagerInstance.GetUIWindow(windowName);
        }

        // These methods are wrappers if UIManager provides them.
        // If UIService should have its own logic, implement here.
        public void AddUICameraToMainCameraStack()
        {
            if (!CheckInitialization()) return;
            uiManagerInstance.AddUICameraToMainCameraStack();
        }

        public void RemoveUICameraFromMainCameraStack()
        {
            if (!CheckInitialization()) return;
            uiManagerInstance.RemoveUICameraFromMainCameraStack();
        }

        public void Dispose()
        {
            UnityEngine.Debug.Log($"{DEBUG_FLAG} Disposing UIService.");
            if (uiManagerInstance != null)
            {
                // Decide if UIService disposing should destroy the UIManager GameObject.
                // If UIManager is a persistent singleton, maybe not.
                // If UIManager is tied to this UIService instance's lifetime, then yes.
                // UnityEngine.Object.Destroy(uiManagerInstance.gameObject);
                uiManagerInstance = null;
            }
            isInitialized = false;
        }

        public (float, float) GetRootCanvasSize()
        {
            if (!CheckInitialization())
            {
                UnityEngine.Debug.LogError($"{DEBUG_FLAG} UIService is not initialized. Operation aborted.");
                return (0, 0);
            }
            return uiManagerInstance.GetRootCanvasSize();
        }
    }
}