using ARPGSample.GameSubSystem;  //  TODO, if use the HotUpdate, the scene managementService must move to AOT
using CycloneGames.GameFramework;
using CycloneGames.Service;
using CycloneGames.UIFramework;
using Zenject;
using Cysharp.Threading.Tasks;

namespace ARPGSample.Gameplay
{
    public class SceneLogic_LaunchScene : SceneLogic
    {
        [Inject] private IAddressablesService addressablesService;
        [Inject] private ISceneManagementService sceneManagementService;
        [Inject] private IUIService uiService;
        [Inject] private IInputService inputService;
        [Inject] private IGraphicsSettingService graphicsSettingService;

        private static readonly string PageName_SimpleLoadingPage = "SimpleLoadingPage";
        private static readonly string PageName_TitlePage = "TitlePage";
        private static readonly string PageName_DialoguePage = "DialoguePage";

        protected override void Start()
        {
            base.Start();
            
            graphicsSettingService.ChangeApplicationFrameRate(60);
            EnterGameScene();
        }

        void EnterGameScene()
        {
            DelayEnterGameScene(100).Forget();
        }

        async UniTask DelayEnterGameScene(int milliSecond)
        {
            await UniTask.Delay(milliSecond);
            sceneManagementService.OpenSceneAsync(new SceneLoadParam[] { new SceneLoadParam()
            {
                SceneKey = "Scene_StartUp",
                Priority = 100
            } }, PageName_SimpleLoadingPage, 100,new []{"Scene_Launch"}, null, () =>
            {
            
            });
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            uiService.CloseUI(PageName_TitlePage);
        }
    }
}

