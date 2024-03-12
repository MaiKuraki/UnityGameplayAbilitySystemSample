using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ARPGSample.Gameplay
{
    public class PlayerAnimationFSM : AnimationFSM
    {
        [SerializeField] private Animator animator;
        private RPGPlayerCharacter ownerPlayer;
        
        protected override void Awake()
        {
            base.Awake();
            
            OwnerChanged += () => ownerPlayer = (RPGPlayerCharacter)GetOwner();
            SyncParam().Forget();
        }

        public async UniTask SimpleFreezeAnimation(float freezeTime = 0.05f)
        {
            animator.speed = 0;
            int freezeTimeMilliSecond = (int)(freezeTime * 1000);
            await UniTask.Delay(freezeTimeMilliSecond);
            animator.speed = 1;
        }
        
        public void EnableAttacking()
        {
            if (ownerPlayer)
            {
                //  NOTE: SetBool will cause AnimationLoop in AttackComboWindow
                animator.SetTrigger(AttackingTrigger);
            }
        }

        public void BreakAttacking()
        {
            if (ownerPlayer)
            {
                animator.SetBool(BreakAttackingTrigger, true);
            }
        }

        public void ResetBreakAttacking()
        {
            if (ownerPlayer)
            {
                animator.SetBool(BreakAttackingTrigger, false);
            }
        }

        async UniTask SyncParam()
        {
            while (true)
            {
                if (ownerPlayer)
                {
                    animator.SetBool(IsInAir, ownerPlayer.IsInAir);
                    animator.SetFloat(VelocityY, ownerPlayer.RB.velocity.y);
                    animator.SetFloat(VelocityX, Mathf.Abs(ownerPlayer.RB.velocity.x));
                }
                await UniTask.DelayFrame(1);
            }
        }
    }
}