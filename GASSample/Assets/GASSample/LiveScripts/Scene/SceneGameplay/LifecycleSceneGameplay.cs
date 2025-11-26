using System;
using System.Threading;
using CycloneGames.UIFramework.Runtime;
using Cysharp.Threading.Tasks;
using GASSample.APIGateway;
using GASSample.UI;
using MackySoft.Navigathena;
using MackySoft.Navigathena.SceneManagement;
using MackySoft.Navigathena.SceneManagement.VContainer;
using VContainer;

namespace GASSample.Scene
{
    public class LifecycleSceneGameplay : ISceneLifecycle
    {
        [Inject] IUIService uiService;
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
            uiService.CloseUI(UIWindowName.GameplayHUD);
            return UniTask.CompletedTask;
        }

        public UniTask OnInitialize(ISceneDataReader reader, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
        {
            // uiService.OpenUI(UIWindowName.GameplayHUD);  //  this already opened in GameplaySceneEntryPoint
            return UniTask.CompletedTask;
        }
    }
}