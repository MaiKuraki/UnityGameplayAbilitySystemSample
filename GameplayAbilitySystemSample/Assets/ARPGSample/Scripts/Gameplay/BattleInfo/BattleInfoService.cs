using CycloneGames.GameFramework;
using UnityEngine;
using Zenject;

namespace ARPGSample.Gameplay
{
    public interface IBattleInfoService
    {
        void AddEnemyHealthBar(Pawn ownerPawn, float InitialVal);
        void RefreshHealthBar(Pawn ownerPawn, float newHealthVal);
        void RemoveEnemyHealthBar(Pawn ownerPawn);
        void ClearAllEnemyHealthBar();
    }
    public class BattleInfoService : IInitializable, IBattleInfoService
    {
        private static readonly string DEBUG_FLAG = "[BattleInfo]";
        [Inject] private DiContainer diContainer;
        
        private BattleInfoSystem battleInfoSystem;
        
        public void Initialize()
        {
            battleInfoSystem = diContainer.InstantiateComponentOnNewGameObject<BattleInfoSystem>("BattleInfoService");
        }

        public void AddEnemyHealthBar(Pawn ownerPawn, float InitialVal)
        {
            battleInfoSystem?.AddEnemyHealthBar(ownerPawn, InitialVal);
        }

        public void RefreshHealthBar(Pawn ownerPawn, float newHealthVal)
        {
            battleInfoSystem?.RefreshHealthBar(ownerPawn, newHealthVal);
        }

        public void RemoveEnemyHealthBar(Pawn ownerPawn)
        {
            battleInfoSystem?.RemoveHealthBar(ownerPawn);
        }

        public void ClearAllEnemyHealthBar()
        {
            battleInfoSystem?.ClearAllHealthBar();
        }
    }
}

