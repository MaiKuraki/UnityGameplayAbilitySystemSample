using UnityEngine;

namespace ARPGSample.Gameplay
{
    public class OverlapChecker : MonoBehaviour
    {
        [SerializeField] protected LayerMask checkLayer;
        [SerializeField] protected Vector2 positionOffset;
        [SerializeField] private bool isEnableCheck;
        public bool IsEnableCheck
        {
            get => isEnableCheck;
            set => isEnableCheck = value;
        }
        
        protected bool isOverlapped;
        public bool IsOverlapped => isOverlapped;
        
        private void FixedUpdate()
        {
            if (IsEnableCheck) CheckOverlap();
        }

        protected virtual void CheckOverlap() { }
        protected virtual void OnDrawGizmosSelected() { }
    }
}