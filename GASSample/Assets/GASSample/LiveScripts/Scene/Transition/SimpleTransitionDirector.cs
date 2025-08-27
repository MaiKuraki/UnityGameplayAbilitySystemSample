using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MackySoft.Navigathena;
using MackySoft.Navigathena.Transitions;
using MackySoft.Navigathena.SceneManagement;
using MackySoft.Navigathena.SceneManagement.Utilities;

namespace GASSample.Scene
{
    public enum ELoadingState
    {
        None,
        Loading,
        Loaded,
    }
    public readonly struct LoadingProgressData
    {
        public float Progress { get; }
        public string Message { get; }
        public ELoadingState State { get; }

        public LoadingProgressData(ELoadingState state, float progress, string message)
        {
            Progress = progress;
            Message = message;
            State = state;
        }
    }
    public class SimpleTransitionDirector : ITransitionDirector
    {
        private readonly ISceneIdentifier m_SceneIdentifier;
        public SimpleTransitionDirector(ISceneIdentifier sceneIdentifier)
        {
            m_SceneIdentifier = sceneIdentifier;
        }

        public ITransitionHandle CreateHandle()
        {
            return new SimpleTransitionHandle(m_SceneIdentifier);
        }

        sealed class SimpleTransitionHandle : ITransitionHandle, IProgress<IProgressDataStore>
        {
            private readonly ISceneIdentifier m_SceneIdentifier;
            private ISceneHandle m_SceneHandle;
            private SimpleTransitionDirectorBehaviour m_Director;

            public SimpleTransitionHandle(ISceneIdentifier sceneIdentifier)
            {
                m_SceneIdentifier = sceneIdentifier;
            }
            public async UniTask Start(CancellationToken cancellation = default)
            {
                var handle = m_SceneIdentifier.CreateHandle();
                UnityEngine.SceneManagement.Scene scene = await handle.Load(cancellationToken: cancellation);
                if (!scene.TryGetComponentInScene(out m_Director, true))
                {
                    throw new InvalidOperationException($"Can not find {nameof(SimpleTransitionDirectorBehaviour)} in the Scene: {scene.name}.");
                }
                m_SceneHandle = handle;
                await m_Director.StartTransition(cancellation);
            }

            public async UniTask End(CancellationToken cancellation = default)
            {
                await m_Director.EndTransition(cancellation);
                m_Director = null;
                await m_SceneHandle.Unload(cancellationToken: cancellation);
            }

            public void Report(IProgressDataStore progressDataStore)
            {
                if (m_Director && progressDataStore.TryGetData(out LoadingProgressData myProgressData))
                {
                    if (myProgressData.Progress > 0.001f)
                    {
                        m_Director.SetPorgressGroupVisibility(true);
                    }

                    if (m_Director.ProgressText) m_Director.ProgressText.text = myProgressData.Progress.ToString("P1");
                    if (m_Director.MessageText) m_Director.MessageText.text = myProgressData.Message;
                    if (m_Director.ProgressSlider) m_Director.ProgressSlider.value = myProgressData.Progress;

                    if (myProgressData.State == ELoadingState.Loaded)
                    {
                        m_Director.SetPorgressGroupVisibility(false);
                        m_Director.SetSpinnerVisibility(false);
                    }
                }
            }
        }
    }
}