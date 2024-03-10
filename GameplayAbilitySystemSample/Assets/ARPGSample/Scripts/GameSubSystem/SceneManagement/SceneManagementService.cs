using System;
using CycloneGames.Service;
using UnityEngine;
using Zenject;

namespace ARPGSample.GameSubSystem
{
    public interface ISceneManagementService
    {
        void OpenSceneAsync(SceneLoadParam[] ScenesForLoad, string LoadingUIKey, int DelayTransferTimeMS = 0,
            string[] UnloadScenes = null, System.Action OnStartLoading = null, System.Action OnFinishedLoading = null);

        bool IsValid { get; }
        void BindStartEvent(System.Action StartEvent);
        void BindFinishedEvent(System.Action FinishedEvent);
    }

    public class SceneManagementService : IInitializable, ISceneManagementService
    {
        [Inject] private IServiceDisplay serviceDisplay;
        [Inject] private DiContainer diContainer;

        private SceneManager sceneManager;

        public void Initialize()
        {
            sceneManager = diContainer.InstantiateComponentOnNewGameObject<SceneManager>("SceneManager");
            sceneManager.transform.SetParent(serviceDisplay.ServiceDisplayTransform);
        }

        public async void OpenSceneAsync(SceneLoadParam[] ScenesForLoad, string LoadingUIKey,
            int DelayTransferTimeMS = 0, string[] UnloadScenes = null, System.Action OnStartLoading = null, System.Action OnFinishedLoading = null)
        {
            if (!IsValid)
            {
                Debug.LogError("Invalid SceneManager");
                return;
            }

            await sceneManager.OpenSceneAsync(ScenesForLoad, LoadingUIKey, DelayTransferTimeMS, UnloadScenes, OnStartLoading, OnFinishedLoading);
        }

        public bool IsValid => sceneManager != null;
        public void BindStartEvent(Action StartEvent)
        {
            sceneManager.OnStartLoadingEvent -= StartEvent;
            sceneManager.OnStartLoadingEvent += StartEvent;
        }

        public void BindFinishedEvent(Action FinishedEvent)
        {
            sceneManager.OnFinishedLoadingEvent -= FinishedEvent;
            sceneManager.OnFinishedLoadingEvent += FinishedEvent;
        }
    }
}