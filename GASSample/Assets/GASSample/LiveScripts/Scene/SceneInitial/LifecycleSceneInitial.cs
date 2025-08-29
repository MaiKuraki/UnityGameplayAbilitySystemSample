using System;
using System.Threading;
using CycloneGames.Logger;
using CycloneGames.Service;
using Cysharp.Threading.Tasks;
using MackySoft.Navigathena;
using MackySoft.Navigathena.SceneManagement;
using MackySoft.Navigathena.SceneManagement.VContainer;
using VContainer;

namespace GASSample.Scene
{
    public class LifecycleSceneInitial : ISceneLifecycle
    {
        private const string DEBUG_FLAG = "[SceneInitial] Navigathena";

        [Inject] IGraphicsSettingService graphicsSettingService;
        public UniTask OnEditorFirstPreInitialize(ISceneDataWriter writer, CancellationToken cancellationToken)
        {
            CLogger.LogInfo($"{DEBUG_FLAG} OnEditorFirstPreInitialize");
            return UniTask.CompletedTask;
        }

        public async UniTask OnEnter(ISceneDataReader reader, CancellationToken cancellationToken)
        {
            CLogger.LogInfo($"{DEBUG_FLAG} OnEnter");
            await GlobalSceneNavigator.Instance.Push(SceneDefinitions.Splash);
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

        public UniTask OnInitialize(ISceneDataReader reader, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
        {
            CLogger.LogInfo($"{DEBUG_FLAG} OnInitialize");

            graphicsSettingService.ChangeRenderResolution(1080);
            graphicsSettingService.ChangeApplicationFrameRate(60);

            return UniTask.CompletedTask;
        }
    }
}