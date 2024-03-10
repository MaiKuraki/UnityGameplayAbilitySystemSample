using ARPGSample.GameSubSystem;
using ARPGSample.UI;
using CycloneGames.GameFramework;
using CycloneGames.UIFramework;
using Zenject;

namespace ARPGSample.Gameplay
{
    public class SceneLogic_StartUp : SceneLogic
    {
        [Inject] private DiContainer diContainer;
        [Inject] private IUIService uiService;
        [Inject] private ISceneManagementService sceneManagementService;
        [Inject] private IInputService inputService;
        

        protected override void Start()
        {
            base.Start();
            
            StartDemo();
        }

        void StartDemo()
        {
            uiService.OpenUI(PageName.StartUpPage, OnPageCreated: BindStartUpPageEvents);
            
            //  Simulate Menu Enter Gameplay Scene
            // EnterGamePlay().Forget();
        }

        void BindStartUpPageEvents(UIPage uiPage)
        {
            StartUpPage startUpPage = uiPage as StartUpPage;
            if (startUpPage)
            {
                startUpPage.OnClickNewGame -= EnterGameplay;
                startUpPage.OnClickNewGame += EnterGameplay;
            }
        }

        void EnterGameplay()
        {
            sceneManagementService.OpenSceneAsync(new SceneLoadParam[] { new SceneLoadParam()
            {
                SceneKey = "Scene_Gameplay",
                Priority = 100
            } }, PageName.SimpleLoadingPage, 100,new []{"Scene_StartUp"}, null, null);
            uiService.CloseUI(PageName.StartUpPage);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
        }
    }
}

