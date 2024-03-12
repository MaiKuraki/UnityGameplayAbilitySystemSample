using CycloneGames.GameFramework;
using UnityEngine;
using Zenject;

namespace ARPGSample.Gameplay
{
    public class RPGPlayerController : PlayerController
    {
        [Inject] private IInputService inputService;
        private RPGPlayerCharacter cmPlayer;
        public override void OnPossess(Pawn InPawn)
        {
            base.OnPossess(InPawn);

            cmPlayer = (RPGPlayerCharacter)InPawn;
            
            inputService.AddVecAction_0(PawnMoveInput);
            inputService.AddBtnAction_1(PawnJumpInput);
            
            inputService.AddBtnAction_0(AttackInput0);      //  PlayerAttack
        }

        public override void OnUnPossess()
        {
            base.OnUnPossess();

            inputService.RemoveVecAction_0(PawnMoveInput);
            inputService.RemoveBtnAction_1(PawnJumpInput);
            inputService.RemoveBtnAction_0(AttackInput0);
            
            cmPlayer = null;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            inputService.RemoveVecAction_0(PawnMoveInput);
            inputService.RemoveBtnAction_1(AttackInput1);
            inputService.RemoveBtnAction_0(AttackInput0);

            cmPlayer = null;
        }

        private void PawnMoveInput(Vector2 MoveVec)
        {
            if(cmPlayer) cmPlayer.MoveInput(MoveVec);
        }

        private void PawnJumpInput()
        {
            if(cmPlayer) cmPlayer.Jump();
        }

        private void AttackInput0()
        {
            if(cmPlayer) cmPlayer.Attack_0();
        }

        private void AttackInput1()
        {
            if(cmPlayer) cmPlayer.Jump();
        }

        private void AttackInput2()
        {
            if(cmPlayer) cmPlayer.Attack_1();
        }
    }
}