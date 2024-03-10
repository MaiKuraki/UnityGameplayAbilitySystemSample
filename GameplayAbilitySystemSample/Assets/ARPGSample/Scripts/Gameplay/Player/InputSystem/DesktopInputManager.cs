using ARPGSample.GameSubSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace ARPGSample.Gameplay
{
    public class DesktopInputManager : InputManagerBase, IInputManager
    {
        private static readonly string DEBUG_FLAG = "[DesktopInput]";

        [Inject] private ISceneManagementService sceneManagementService;
        private PlayerInputControls playerInputControls;

        private void Awake()
        {
            playerInputControls = new PlayerInputControls();
            
            playerInputControls.Gameplay.Movement.performed += PerformedMovement;
            playerInputControls.Gameplay.Movement.canceled += CancelMovement;
            playerInputControls.Gameplay.Jump.performed += JumpInput;
            playerInputControls.Gameplay.Attack0.performed += Attack_0;
            playerInputControls.Gameplay.Attack1.performed += Attack_1;
        }

        private void PerformedMovement(InputAction.CallbackContext IA)
        {
            UpdateVec_0(IA.ReadValue<Vector2>());
        }

        private void CancelMovement(InputAction.CallbackContext IA)
        {
            UpdateVec_0(Vector2.zero);
        }

        private void JumpInput(InputAction.CallbackContext IA)
        {
            ClickBtn_1();
        }
        
        private void Attack_0(InputAction.CallbackContext IA)
        {
            ClickBtn_0();
        }
        
        private void Attack_1(InputAction.CallbackContext IA)
        {
            ClickBtn_2();
        }
        
        private void ClickBtn_3(InputAction.CallbackContext IA)
        {
            ClickBtn_3();
        }

        public override void EnableInput()
        {
            playerInputControls.Enable();
        }

        public override void BlockInput()
        {
            playerInputControls.Disable();
        }
    }
}