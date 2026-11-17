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
    /// Runs once after the LiveContent root is spawned: screen setup, persistent prefabs, input, then
    /// the first scene load.
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
            // Keep the screen black until the first scene is ready.
            ScreenFade.CoverNow();

            Screen.SetResolution(1920, 1080, Screen.fullScreen);
            Application.targetFrameRate = 60;

            // The AOT bootstrapper already initialized the package to load this prefab.
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

            // InputUser is a device whitelist: complete it for reconnected and late-connected gamepads.
            InputDeviceSync.Install(inputPlayer);
        }
    }
}
