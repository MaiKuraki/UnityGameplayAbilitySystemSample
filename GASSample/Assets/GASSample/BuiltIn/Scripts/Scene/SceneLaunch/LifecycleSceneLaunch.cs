using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MackySoft.Navigathena;
using MackySoft.Navigathena.SceneManagement;
using MackySoft.Navigathena.SceneManagement.VContainer;

namespace GASSample.AOT
{
    /// <summary>
    /// The built-in launch scene does one thing: push Scene_Initial, which owns all the bootstrap work.
    /// </summary>
    public class LifecycleSceneLaunch : ISceneLifecycle
    {
        public UniTask OnEditorFirstPreInitialize(ISceneDataWriter writer, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnInitialize(ISceneDataReader reader, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnEnter(ISceneDataReader reader, CancellationToken cancellationToken)
        {
            return GlobalSceneNavigator.Instance.Push(BuiltInSceneDefinitions.Initial);
        }

        public UniTask OnExit(ISceneDataWriter writer, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnFinalize(ISceneDataWriter writer, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}
