using CycloneGames.GameFramework;

namespace ARPGSample.Gameplay
{
    public class BreakAttackState : IAttackState
    {
        private static readonly string STATE_NAME = "[BreakAttack]";
        public void OnEnter(Pawn pawn)
        {
            UnityEngine.Debug.Log($"{STATE_NAME} Enter");
            var rpgPawn = (RPGPawn)pawn;
            if (rpgPawn)
            {
                rpgPawn.AnimationFsm.BreakAttacking();
            }
        }

        public void OnExit(Pawn pawn)
        {
            UnityEngine.Debug.Log($"{STATE_NAME} Exit");
        }

        public void OnUpdate(Pawn pawn)
        {
            
        }

        public void Break(Pawn pawn)
        {
            
        }

        public void ComboWindow(Pawn pawn)
        {
            
        }
    }
}