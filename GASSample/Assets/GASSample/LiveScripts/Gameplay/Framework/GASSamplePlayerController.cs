using CycloneGames.GameplayFramework.Runtime;
using UnityEngine;

namespace GASSample.Gameplay
{
    public class GASSamplePlayerController : PlayerController
    {
        [SerializeField] private float followDistance = 3f;
        [SerializeField] private float pivotHeight = 1.6f;
        [SerializeField] private float lookAtHeight = 1.1f;
        [SerializeField] private float overrideFov = 60f;
        
        private readonly GASSampleThirdPersonCameraMode thirdPersonMode = new GASSampleThirdPersonCameraMode();

        protected override CameraMode CreateDefaultCameraMode()
        {
            return new ViewTargetCameraMode();
        }

        protected override void OnPossess(Pawn InPawn)
        {
            base.OnPossess(InPawn);

            EnterGameplayCamera();
        }

        public override void UnPossess()
        {
            base.UnPossess();

            SetBaseCameraMode(new ViewTargetCameraMode());
        }

        private void EnterGameplayCamera()
        {
            thirdPersonMode.FollowDistance = followDistance;
            thirdPersonMode.PivotHeight = pivotHeight;
            thirdPersonMode.LookAtHeight = lookAtHeight;
            thirdPersonMode.OverrideFov = overrideFov;
            SetBaseCameraMode(thirdPersonMode);
        }
    }
}
