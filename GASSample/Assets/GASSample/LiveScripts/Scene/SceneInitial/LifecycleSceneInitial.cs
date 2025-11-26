using System;
using System.Threading;
using CycloneGames.AssetManagement.Runtime;
using CycloneGames.AssetManagement.Runtime.Integrations.Navigathena;
using CycloneGames.Factory.Runtime;
using CycloneGames.Logger;
using CycloneGames.Service.Runtime;
using CycloneGames.UIFramework.Runtime;
using Cysharp.Threading.Tasks;
using GASSample.AssetManagement;
using MackySoft.Navigathena;
using MackySoft.Navigathena.SceneManagement;
using MackySoft.Navigathena.SceneManagement.VContainer;
using UnityEngine.SceneManagement;
using VContainer;

namespace GASSample.Scene
{
    public class LifecycleSceneInitial : ISceneLifecycle
    {
        private const string DEBUG_FLAG = "[SceneInitial] Navigathena";

        [Inject] IGraphicsSettingService graphicsSettingService;
        [Inject] AssetResolverForDontDestroy assetResolver;
        [Inject] IAssetModule assetModule;
        [Inject] IAssetPathBuilderFactory assetPathFactory;
        [Inject] IUnityObjectSpawner objectSpawner;
        [Inject] IMainCameraService cameraService;
        [Inject] IUIService uiService;
        public UniTask OnEditorFirstPreInitialize(ISceneDataWriter writer, CancellationToken cancellationToken)
        {
            CLogger.LogInfo($"{DEBUG_FLAG} OnEditorFirstPreInitialize");
            return UniTask.CompletedTask;
        }

        public UniTask OnEnter(ISceneDataReader reader, CancellationToken cancellationToken)
        {
            CLogger.LogInfo($"{DEBUG_FLAG} OnEnter");
            return UniTask.CompletedTask;
        }

        public UniTask OnExit(ISceneDataWriter writer, CancellationToken cancellationToken)
        {
            CLogger.LogInfo($"{DEBUG_FLAG} OnExit");
            return UniTask.CompletedTask;
        }

        public UniTask OnFinalize(ISceneDataWriter writer, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
        {
            CLogger.LogInfo($"{DEBUG_FLAG} OnFinalize");
            return UniTask.CompletedTask;
        }

        public async UniTask OnInitialize(ISceneDataReader reader, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
        {
            CLogger.LogInfo($"{DEBUG_FLAG} OnInitialize");

            var pkg = assetModule.GetPackage(AssetPackageName.DefaultPackage);

            graphicsSettingService.ChangeRenderResolution(1080);
            graphicsSettingService.ChangeApplicationFrameRate(60);

            await assetResolver.InitializeAsync(assetModule);

            uiService.Initialize(assetPathFactory, objectSpawner, cameraService, pkg);

            AssetManagementSceneIdentifier sceneSplash = new AssetManagementSceneIdentifier(pkg, ScenePath.Splash, LoadSceneMode.Single, true);
            await GlobalSceneNavigator.Instance.Push(sceneSplash, interruptOperation: new UnloadPackageAssetsOperation(pkg));
        }
    }
}