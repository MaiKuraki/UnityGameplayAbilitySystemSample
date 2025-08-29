using CycloneGames.UIFramework;
using GASSample.APIGateway;
using GASSample.Scene;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace GASSample.UI
{
    public class UIWindowGameplayHUD : UIWindow
    {
        [Inject] ISceneManagementAPIGateway sceneManagementAPI;
        [SerializeField] Button Btn_Back;

        protected override void Awake()
        {
            base.Awake();

            Btn_Back.OnClickAsObservable().Subscribe(_ => BackToTitle());
        }

        void BackToTitle()
        {
            sceneManagementAPI.Push(SceneDefinitions.Title);
        }
    }
}