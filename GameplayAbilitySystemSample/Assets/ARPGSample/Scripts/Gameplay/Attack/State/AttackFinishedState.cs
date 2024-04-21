using CycloneGames.GameFramework;

namespace ARPGSample.Gameplay
{
    public class AttackFinishedState : AttackState
    {
        private static readonly string STATE_NAME = "[AttackFinished]";

        public override void OnEnter(Pawn pawn)
        {
            UnityEngine.Debug.Log($"{STATE_NAME} Enter");
            var rpgPawn = (RPGPlayerCharacter)pawn;
            if (rpgPawn)
            {
                rpgPawn.AnimationFsm.BreakAttacking();
            }
        }

        public override void OnExit(Pawn pawn)
        {
            UnityEngine.Debug.Log($"{STATE_NAME} Exit");
        }
    }
}