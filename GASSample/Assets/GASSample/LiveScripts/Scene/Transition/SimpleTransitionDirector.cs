using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MackySoft.Navigathena;
using MackySoft.Navigathena.Transitions;
using MackySoft.Navigathena.SceneManagement;
using MackySoft.Navigathena.SceneManagement.Utilities;
using CycloneGames.AssetManagement.Runtime;
using CycloneGames.Logger;
using GASSample.AssetManagement;

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
    public readonly struct TransitionDisplayData
    {
        public float EnterTransitionDuration { get; }
        public float ExitTransitionDuration { get; }
        public TransitionDisplayData(float EnterTransitionDuration, float ExitTransitionDuration)
        {
            this.EnterTransitionDuration = EnterTransitionDuration;
            this.ExitTransitionDuration = ExitTransitionDuration;
        }
    }
    public class SimpleTransitionDirector : ITransitionDirector
    {
        private readonly ISceneIdentifier m_SceneIdentifier;
        private readonly IAssetModule assetModule;
        private readonly TransitionDisplayData transitionDisplayData;
        public SimpleTransitionDirector(IAssetModule assetModule, ISceneIdentifier sceneIdentifier, TransitionDisplayData transitionDisplayData)
        {
            this.assetModule = assetModule;
            m_SceneIdentifier = sceneIdentifier;
            this.transitionDisplayData = transitionDisplayData;
        }

        public ITransitionHandle CreateHandle()
        {
            return new SimpleTransitionHandle(assetModule, m_SceneIdentifier, transitionDisplayData);
        }

        sealed class SimpleTransitionHandle : ITransitionHandle, IProgress<IProgressDataStore>
        {
            private const string DEBUG_FLAG = "[SimpleTransitionHandle]";
            private IAssetModule assetModule;
            private readonly ISceneIdentifier m_SceneIdentifier;
            private MackySoft.Navigathena.SceneManagement.ISceneHandle m_SceneHandle;
            private SimpleTransitionDirectorBehaviour m_Director;
            private readonly TransitionDisplayData transitionDisplayData;

            public SimpleTransitionHandle(IAssetModule assetModule, ISceneIdentifier sceneIdentifier, TransitionDisplayData transitionDisplayData)
            {
                this.assetModule = assetModule;
                m_SceneIdentifier = sceneIdentifier;
                this.transitionDisplayData = transitionDisplayData;
            }
            public async UniTask Start(CancellationToken cancellation = default)
            {
                var handle = m_SceneIdentifier.CreateHandle();
                UnityEngine.SceneManagement.Scene scene = await handle.Load(cancellationToken: cancellation);
                if (!scene.TryGetComponentInScene(out m_Director, true))
                {
                    throw new InvalidOperationException($"Can not find {nameof(SimpleTransitionDirectorBehaviour)} in the Scene: {scene.name}.");
                }
                m_Director.InitTransitionData(transitionDisplayData.EnterTransitionDuration, transitionDisplayData.ExitTransitionDuration);
                m_SceneHandle = handle;
                await m_Director.StartTransition(cancellation);
            }

            public async UniTask End(CancellationToken cancellation = default)
            {
                await m_Director.EndTransition(cancellation);
                m_Director = null;
                await m_SceneHandle.Unload(cancellationToken: cancellation);
                await assetModule.GetPackage(AssetPackageName.DefaultPackage).UnloadUnusedAssetsAsync();
                CLogger.LogInfo($"{DEBUG_FLAG} Unload unused asset");
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