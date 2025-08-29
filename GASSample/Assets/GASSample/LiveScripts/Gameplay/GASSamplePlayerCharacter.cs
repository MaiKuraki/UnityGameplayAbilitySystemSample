using CycloneGames.GameplayFramework;
using UnityEngine;
using VContainer;

namespace GASSample.Gameplay
{
    public class GASSamplePlayerCharacter : GASSampleCharacter
    {
        [Inject] IWorld world;

        [SerializeField] private Transform CameraFocusTF;

        override protected void Update()
        {
            GetMovementComponent?.MoveWithVelocity(movementVelocity);
        }

        public override void PossessedBy(Controller NewController)
        {
            base.PossessedBy(NewController);

            PlayerController pc = NewController as PlayerController;

            var cameraManager = pc.GetCameraManager();
            cameraManager.SetViewTarget(CameraFocusTF);

            movementVelocity = transform.forward;
        }
    }
}
