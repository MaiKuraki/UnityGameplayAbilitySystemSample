using System;
using System.Threading;
using CycloneGames.UIFramework;
using Cysharp.Threading.Tasks;
using MackySoft.Navigathena;
using MackySoft.Navigathena.SceneManagement;
using MackySoft.Navigathena.SceneManagement.VContainer;
using VContainer;
using GASSample.UI;

namespace GASSample.Scene
{
    public class LifecycleSceneTitle : ISceneLifecycle
    {
        private const string DEBUG_FLAG = "[LifecycleSceneTitle]";

        [Inject] private readonly IUIService uiService;

        public UniTask OnEditorFirstPreInitialize(ISceneDataWriter writer, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnEnter(ISceneDataReader reader, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnExit(ISceneDataWriter writer, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnFinalize(ISceneDataWriter writer, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
        {
            uiService.CloseUI(UIWindowName.Title);
            return UniTask.CompletedTask;
        }

        public async UniTask OnInitialize(ISceneDataReader reader, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
        {
            uiService.OpenUI(UIWindowName.Title);
            await UpdateProgress(progress, cancellationToken);
        }

        private async UniTask UpdateProgress(IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
        {
            ProgressDataStore<LoadingProgressData> store = new();
            int fakeProgress = 0;
            int targetProgress = 100;
            int step = 4;
            while (fakeProgress < targetProgress)
            {
                fakeProgress += step;
                progress.Report(store.SetData(new LoadingProgressData(ELoadingState.Loading, fakeProgress / (float)targetProgress, "Loading...")));
                await UniTask.Delay(30);
            }
            progress.Report(store.SetData(new LoadingProgressData(ELoadingState.Loaded, 1f, "Complete")));
            await UniTask.Delay(50);
            await UniTask.CompletedTask;
        }
    }
}