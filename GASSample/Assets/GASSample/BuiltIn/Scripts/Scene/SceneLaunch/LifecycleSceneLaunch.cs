using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MackySoft.Navigathena;
using MackySoft.Navigathena.SceneManagement;
using MackySoft.Navigathena.SceneManagement.VContainer;

namespace GASSample.AOT
{
    public class LifecycleSceneLaunch : ISceneLifecycle
    {
        private const string DEBUG_FLAG = "[LifecycleSceneLaunch]";
        public UniTask OnEditorFirstPreInitialize(ISceneDataWriter writer, CancellationToken cancellationToken)
        {
            UnityEngine.Debug.LogWarning($"{DEBUG_FLAG} OnEditorFirstPreInitialize");
            return UniTask.CompletedTask;
        }

        public async UniTask OnEnter(ISceneDataReader reader, CancellationToken cancellationToken)
        {
            UnityEngine.Debug.LogWarning($"{DEBUG_FLAG} OnEnter");
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

        public UniTask OnInitialize(ISceneDataReader reader, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
        {
            UnityEngine.Debug.LogWarning($"{DEBUG_FLAG} OnInitialize");
            return UniTask.CompletedTask;
        }
    }
}