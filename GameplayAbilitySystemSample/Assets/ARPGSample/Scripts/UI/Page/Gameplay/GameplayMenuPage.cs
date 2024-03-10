using ARPGSample.GameSubSystem;
using CycloneGames.UIFramework;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ARPGSample.UI
{
    public class GameplayMenuPage : UIPage
    {
        [Inject] private ISceneManagementService sceneManagementService;
        [Inject] private IUIService uiService;
        
        [SerializeField] private Button Btn_BackToStartUp;

        protected override void Awake()
        {
            base.Awake();
            
            Btn_BackToStartUp?.onClick.AddListener(BackToStartUpScene);
        }

        void BackToStartUpScene()
        {
            uiService.CloseUI(UI.PageName.GameplayMenuPage);
            sceneManagementService.OpenSceneAsync(new SceneLoadParam[] { new SceneLoadParam()
            {
                SceneKey = "Scene_StartUp",
                Priority = 100
            } }, UI.PageName.SimpleLoadingPage, 100,new []{"Scene_Gameplay"}, null, () =>
            {
                
            });
        }
    }
}