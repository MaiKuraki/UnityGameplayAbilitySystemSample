using Cinemachine;
using UnityEngine;

namespace CycloneGames.GameFramework
{
    public class CameraManager : Actor
    {
        private readonly static string DEBUG_FLAG = "[Camera Manager]";
        
        [SerializeField] protected CinemachineVirtualCamera VirtualCamera;
        [SerializeField] private float DefaultFOV = 60.0f;
        
        private PlayerController PCOwner;
        private float lockedFOV;
        public float GetLockedFOV() => lockedFOV;
        public void SetFOV(float NewFOV)
        {
            lockedFOV = NewFOV;
            VirtualCamera.m_Lens.FieldOfView = lockedFOV;
        }
        private Actor PendingViewTarget;

        public void SetViewTarget(Actor NewTarget)
        {
            PendingViewTarget = NewTarget;
            VirtualCamera.Follow = PendingViewTarget.transform;
            VirtualCamera.LookAt = PendingViewTarget.transform;
        }

        public virtual void InitializeFor(PlayerController PlayerController)
        {
            SetFOV(DefaultFOV);
            
            PCOwner = PlayerController;
            
            SetViewTarget(PlayerController);
        }

        protected override void Awake()
        {
            base.Awake();

            Debug.LogWarning($"{DEBUG_FLAG} Your working camera for CameraManager must have a 'CinemachineBrain' component, this is just a notice");
        }
    }
}
