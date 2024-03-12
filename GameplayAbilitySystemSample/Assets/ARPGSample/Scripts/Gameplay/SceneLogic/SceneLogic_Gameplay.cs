using ARPGSample.UI;
using CycloneGames.GameFramework;
using CycloneGames.UIFramework;
using Zenject;

namespace ARPGSample.Gameplay
{
    public class SceneLogic_Gameplay : SceneLogic
    {
        [Inject] private IUIService uiService;
        
        [Inject] private IInputService inputService;

        [Inject] private IDialogueService dialogueService;
        [Inject] private IWorld world;
        
        private DialoguePage dp;
        protected override void Start()
        {
            base.Start();
            
            uiService.OpenUI(PageName.GameplayMenuPage);
            uiService.OpenUI(PageName.HUDPage);
            uiService.OpenUI(UI.PageName.BattleInfoPage);
            
            dialogueService.StartDialogue(DialogueTarget.WorldStory, "GameplayStart", OnDialogueStartEvent: () =>
                {
                    inputService.SetInputBlockState(new IInputService.InputBlockHandler(GetType().Name, true));
                    uiService.CloseUI(PageName.HUDPage);
                },
                OnDialogueFinishedEvent: () =>
                {
                    uiService.CloseUI(PageName.DialoguePage);
                    inputService.SetInputBlockState(new IInputService.InputBlockHandler(GetType().Name, false));
                    uiService.OpenUI(PageName.HUDPage, page =>
                    {
                        RefreshHUD();
                    });
                });
        }
        private void RefreshHUD()
        {
            var rpgPawn = (RPGPlayerCharacter)world.GetPlayerPawn();
            if (rpgPawn)
            {
                rpgPawn.RefreshAttributesUI();
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            uiService?.CloseUI(PageName.DialoguePage);
            uiService?.CloseUI(PageName.HUDPage);
            uiService?.CloseUI(UI.PageName.BattleInfoPage);
        }
    }
}