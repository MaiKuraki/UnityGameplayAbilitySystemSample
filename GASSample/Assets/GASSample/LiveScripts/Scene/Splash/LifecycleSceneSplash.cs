using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MackySoft.Navigathena;
using MackySoft.Navigathena.SceneManagement;
using MackySoft.Navigathena.SceneManagement.VContainer;

namespace GASSample.Scene
{
    public class LifecycleSceneSplash : ISceneLifecycle
    {
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
            await GlobalSceneNavigator.Instance.Push(SceneDefinitions.Title, transitionDirector: new SimpleTransitionDirector(SceneDefinitions.Transition));
        }
    }
}