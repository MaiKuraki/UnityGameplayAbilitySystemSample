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

namespace GASSample.Scene
{
    /// <summary>
    /// Bootstrap scene. It prepares the screen and input, spawns the objects that must survive scene
    /// changes, then pushes the splash scene.
    /// </summary>
    public class LifecycleSceneInitial : ISceneLifecycle
    {
        [Inject] private AssetResolverForDontDestroy assetResolver;
        [Inject] private IAssetModule assetModule;

        public UniTask OnEditorFirstPreInitialize(ISceneDataWriter writer, CancellationToken cancellationToken) => UniTask.CompletedTask;

        public UniTask OnInitialize(ISceneDataReader reader, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
        {
            return BootstrapAsync();
        }

        public UniTask OnEnter(ISceneDataReader reader, CancellationToken cancellationToken) => UniTask.CompletedTask;

        public UniTask OnExit(ISceneDataWriter writer, CancellationToken cancellationToken) => UniTask.CompletedTask;

        public UniTask OnFinalize(ISceneDataWriter writer, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken) => UniTask.CompletedTask;

        private async UniTask BootstrapAsync()
        {
            var pkg = assetModule.GetPackage(AssetPackageName.DefaultPackage);

            Screen.SetResolution(1920, 1080, Screen.fullScreen);
            Application.targetFrameRate = 60;

            await assetResolver.InitializeAsync(assetModule);
            await InitializeInputAsync();

            var sceneLoader = (IAssetSceneLoader)pkg;
            var sceneSplash = new AssetManagementSceneIdentifier(sceneLoader, ScenePath.Splash, LoadSceneMode.Additive, true);
            await GlobalSceneNavigator.Instance.Push(sceneSplash, interruptOperation: new UnloadPackageAssetsOperation(pkg));
        }

        /// <summary>
        /// Loads the YAML input configuration from StreamingAssets and joins local player 0.
        /// </summary>
        private static async UniTask InitializeInputAsync()
        {
            var manager = InputManager.Instance;
            if (!manager.IsInitialized)
            {
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
                        $"Input initialization failed: {result.Status} {result.Error}");
                }
            }

            var inputPlayer = manager.GetInputPlayer(0) ?? manager.JoinSinglePlayer(0);
            if (inputPlayer == null)
            {
                throw new InvalidOperationException("Local player 0 could not join with any device.");
            }

            // A player only answers to devices paired to its InputUser; keep that list complete for
            // reconnected controllers and for gamepads plugged in after the join.
            InputDeviceSync.Install(inputPlayer);
        }
    }
}
