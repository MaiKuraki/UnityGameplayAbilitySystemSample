using CycloneGames.GameFramework;
using CycloneGames.UIFramework;
using Cysharp.Threading.Tasks;
using ARPGSample.UI;
using UnityEngine;
using Zenject;

namespace ARPGSample.Gameplay
{
    public class BattleInfoSystem : MonoBehaviour
    {
        [Inject] private IUIService uiService;

        private BattleInfoPage battleInfoPage;
        private bool IsBattleInfoPageOpening = false;
        
        public void AddEnemyHealthBar(Pawn ownerPawn, float newHealthVal)
        {
            battleInfoPage = uiService.GetUIPage(UI.PageName.BattleInfoPage) as BattleInfoPage;
            if (!battleInfoPage)
            {
                if (!IsBattleInfoPageOpening)
                {
                    IsBattleInfoPageOpening = true;
                    uiService.OpenUI(UI.PageName.BattleInfoPage, OnBattleInfoPageCreated);
                }
            }
            
            AddEnemyHealthBarAsync( ownerPawn, newHealthVal).Forget();
        }
        
        async UniTask AddEnemyHealthBarAsync(Pawn ownerPawn, float newHealthVal)
        {
            await UniTask.WaitUntil(() => battleInfoPage != null);
            battleInfoPage.AddEnemyHealthBar(ownerPawn, newHealthVal);
        }

        public void RefreshHealthBar(Pawn ownerPawn, float newHealthVal)
        {
            battleInfoPage?.RefreshHealthBar(ownerPawn, newHealthVal);
        }

        public void RemoveHealthBar(Pawn pawn)
        {
            battleInfoPage?.RemoveHealthBar(pawn);
        }

        public void ClearAllHealthBar()
        {
            battleInfoPage?.ClearAllHealthBar();
        }
        
        void OnBattleInfoPageCreated(UIPage page)
        {
            battleInfoPage = page as BattleInfoPage;
            IsBattleInfoPageOpening = false;
        }

        private void OnDestroy()
        {
            ClearAllHealthBar();
            battleInfoPage = null;
        }
    }
}
