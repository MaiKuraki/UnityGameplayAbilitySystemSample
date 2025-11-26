using System;
using System.Threading;
using CycloneGames.AssetManagement.Runtime;
using CycloneGames.AssetManagement.Runtime.Integrations.Navigathena;
using Cysharp.Threading.Tasks;
using GASSample.APIGateway;
using GASSample.AssetManagement;
using MackySoft.Navigathena;
using MackySoft.Navigathena.SceneManagement;
using MackySoft.Navigathena.SceneManagement.VContainer;
using UnityEngine.SceneManagement;
using VContainer;

namespace GASSample.Scene
{
    public class LifecycleSceneSplash : ISceneLifecycle
    {
        [Inject] IAssetModule assetModule;
        [Inject] ISceneManagementAPIGateway sceneManagementAPIGateway;
        public UniTask OnEditorFirstPreInitialize(ISceneDataWriter writer, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public async UniTask OnEnter(ISceneDataReader reader, CancellationToken cancellationToken)
        {
            await EnterNextScene(cancellationToken);
        }

        public UniTask OnExit(ISceneDataWriter writer, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnFinalize(ISceneDataWriter writer, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnInitialize(ISceneDataReader reader, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        private async UniTask EnterNextScene(CancellationToken cancellation)
        {
            int SplashDuration = 3000; // ms
            await UniTask.Delay(SplashDuration, true, PlayerLoopTiming.FixedUpdate, cancellation);

            var pkg = assetModule.GetPackage(AssetPackageName.DefaultPackage);
            AssetManagementSceneIdentifier sceneTitle = new AssetManagementSceneIdentifier(pkg, ScenePath.Title, LoadSceneMode.Additive, true);
            AssetManagementSceneIdentifier sceneTransition = new AssetManagementSceneIdentifier(pkg, ScenePath.Transition, LoadSceneMode.Additive, true);
            await GlobalSceneNavigator.Instance.Push(sceneTitle, interruptOperation: new UnloadPackageAssetsOperation(pkg), transitionDirector: new SimpleTransitionDirector(assetModule, sceneTransition,  new TransitionDisplayData(0.25f, 0.25f)));
        }
    }
}