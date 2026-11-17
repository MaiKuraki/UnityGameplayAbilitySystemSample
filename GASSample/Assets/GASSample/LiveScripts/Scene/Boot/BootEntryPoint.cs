using System;
using System.Threading;
using CycloneGames.AssetManagement.Runtime;
using CycloneGames.InputSystem.Runtime;
using CycloneGames.IO.Unity;
using Cysharp.Threading.Tasks;
using GASSample.AssetManagement;
using GASSample.Input;
using UnityEngine;
using VContainer.Unity;

namespace GASSample.Scene
{
    /// <summary>
    /// Runs once from the launch scene. It configures the screen, creates the objects that must outlive
    /// every scene change (camera, UI, debug tools), initializes input, and then enters the scene flow.
    /// </summary>
    public class BootEntryPoint : IAsyncStartable
    {
        private readonly IAssetModule assetModule;
        private readonly AssetResolverForDontDestroy assetResolver;

        public BootEntryPoint(IAssetModule assetModule, AssetResolverForDontDestroy assetResolver)
        {
            this.assetModule = assetModule;
            this.assetResolver = assetResolver;
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            // The launch scene is empty, so hide it before the first frame is drawn.
            ScreenFade.CoverNow();

            Screen.SetResolution(1920, 1080, Screen.fullScreen);
            Application.targetFrameRate = 60;

            await AssetManager.InitializeDefaultPackageAsync(
                assetModule,
                AssetPackageName.DefaultPackage,
                new AssetManagementOptions(),
                new AssetPackageInitOptions());

            await assetResolver.InitializeAsync(assetModule);

            await InitializeInputAsync();

            await SceneFlow.LoadAsync(assetModule, SceneFlow.Title);
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
