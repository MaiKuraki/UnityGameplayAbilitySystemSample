using System;
using System.Threading;
using CycloneGames.AssetManagement.Runtime;
using CycloneGames.AssetManagement.Runtime.Integrations.Navigathena;
using CycloneGames.InputSystem.Runtime;
using CycloneGames.IO.Unity;
using Cysharp.Threading.Tasks;
using GASSample.AssetManagement;
using GASSample.Input;
using MackySoft.Navigathena;
using MackySoft.Navigathena.SceneManagement;
using MackySoft.Navigathena.SceneManagement.VContainer;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using CycloneGames.Logging;

namespace GASSample.Scene
{
    public class LifecycleSceneInitial : ISceneLifecycle
    {
        private static readonly LogChannel Log = LogChannel.Create("GASSample");
        private const string DEBUG_FLAG = "[SceneInitial] Navigathena";

        [Inject] AssetResolverForDontDestroy assetResolver;
        [Inject] IAssetModule assetModule;
        public UniTask OnEditorFirstPreInitialize(ISceneDataWriter writer, CancellationToken cancellationToken)
        {
            Log.Info($"{DEBUG_FLAG} OnEditorFirstPreInitialize");
            return UniTask.CompletedTask;
        }

        public UniTask OnEnter(ISceneDataReader reader, CancellationToken cancellationToken)
        {
            Log.Info($"{DEBUG_FLAG} OnEnter");
            return UniTask.CompletedTask;
        }

        public UniTask OnExit(ISceneDataWriter writer, CancellationToken cancellationToken)
        {
            Log.Info($"{DEBUG_FLAG} OnExit");
            return UniTask.CompletedTask;
        }

        public UniTask OnFinalize(ISceneDataWriter writer, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
        {
            Log.Info($"{DEBUG_FLAG} OnFinalize");
            return UniTask.CompletedTask;
        }

        public async UniTask OnInitialize(ISceneDataReader reader, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
        {
            Log.Info($"{DEBUG_FLAG} OnInitialize");

            var pkg = assetModule.GetPackage(AssetPackageName.DefaultPackage);

            ApplyShortEdgeResolution(1080);
            Application.targetFrameRate = 60;

            await assetResolver.InitializeAsync(assetModule);
            await InitializeInputAsync();

            var sceneLoader = (IAssetSceneLoader)pkg;
            AssetManagementSceneIdentifier sceneSplash = new AssetManagementSceneIdentifier(sceneLoader, ScenePath.Splash, LoadSceneMode.Additive, true);
            await GlobalSceneNavigator.Instance.Push(sceneSplash, interruptOperation: new UnloadPackageAssetsOperation(pkg));
        }

        /// <summary>
        /// Loads the YAML input configuration from StreamingAssets and joins local player 0.
        /// The config pairs every matching device (keyboard/mouse and gamepad live side by side),
        /// so switching between them needs no re-join.
        /// </summary>
        private static async UniTask InitializeInputAsync()
        {
            InputManager manager = InputManager.Instance;
            if (manager.IsInitialized)
            {
                return;
            }

            string configUri = UnityFileUri.Create("input_config.yaml", UnityFileLocation.StreamingAssets);
            InputSystemLoadResult result = await InputSystemLoader.LoadAndInitializeAsync(
                new UriInputConfigurationSource(),
                configUri,
                userStore: null,
                userKey: null,
                manager);

            if (!result.IsSuccess)
            {
                throw new InvalidOperationException(
                    $"{DEBUG_FLAG} Input initialization failed: {result.Status} {result.Error}");
            }

            IInputPlayer inputPlayer = manager.GetInputPlayer(0) ?? manager.JoinSinglePlayer(0);
            if (inputPlayer == null)
            {
                throw new InvalidOperationException($"{DEBUG_FLAG} Local player 0 could not join with any device.");
            }

            // The player only answers to devices paired to its InputUser; keep that list complete for
            // re-paired controllers and for gamepads plugged in after the join.
            InputDeviceSync.Install(inputPlayer);
        }

        /// <summary>
        /// Sets a window that keeps the requested short edge and the display aspect ratio.
        /// Replaces the resolution helper that used to live in the removed CycloneGames.Services package.
        /// </summary>
        private static void ApplyShortEdgeResolution(int shortEdge)
        {
            int displayWidth = Display.main.systemWidth;
            int displayHeight = Display.main.systemHeight;
            float aspect = displayWidth > 0 && displayHeight > 0
                ? (float)displayWidth / displayHeight
                : 16f / 9f;

            int width = Mathf.RoundToInt(shortEdge * aspect);
            Screen.SetResolution(Mathf.Max(1, width), Mathf.Max(1, shortEdge), Screen.fullScreen);
        }
    }
}