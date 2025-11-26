using System;
using System.Threading;
using CycloneGames.AssetManagement.Runtime;
using Cysharp.Threading.Tasks;
using MackySoft.Navigathena;
using MackySoft.Navigathena.SceneManagement;
using MackySoft.Navigathena.SceneManagement.VContainer;
using VContainer;

namespace GASSample.AOT
{
    public class LifecycleSceneLaunch : ISceneLifecycle
    {
        private const string DEBUG_FLAG = "[LifecycleSceneLaunch]";
        [Inject] IAssetModule assetModule;
        private const string DefaultPackage = "DefaultPackage";

        public UniTask OnEditorFirstPreInitialize(ISceneDataWriter writer, CancellationToken cancellationToken)
        {
            UnityEngine.Debug.LogWarning($"{DEBUG_FLAG} OnEditorFirstPreInitialize");
            return UniTask.CompletedTask;
        }

        public async UniTask OnEnter(ISceneDataReader reader, CancellationToken cancellationToken)
        {
            UnityEngine.Debug.LogWarning($"{DEBUG_FLAG} OnEnter");

            var pkg = assetModule.GetPackage(DefaultPackage);

            await GlobalSceneNavigator.Instance.Push(BuiltInSceneDefinitions.Initial);
        }

        public UniTask OnExit(ISceneDataWriter writer, CancellationToken cancellationToken)
        {
            UnityEngine.Debug.LogWarning($"{DEBUG_FLAG} OnExit");
            return UniTask.CompletedTask;
        }

        public UniTask OnFinalize(ISceneDataWriter writer, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
        {
            UnityEngine.Debug.LogWarning($"{DEBUG_FLAG} OnFinalize");
            return UniTask.CompletedTask;
        }

        public async UniTask OnInitialize(ISceneDataReader reader, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
        {
            UnityEngine.Debug.LogWarning($"{DEBUG_FLAG} OnInitialize");
            await UniTask.CompletedTask;
        }
    }
}