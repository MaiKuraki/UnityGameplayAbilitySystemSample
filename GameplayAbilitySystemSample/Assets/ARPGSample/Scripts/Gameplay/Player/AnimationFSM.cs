using CycloneGames.GameFramework;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ARPGSample.Gameplay
{
    public class AnimationFSM : Actor
    {
        [SerializeField] private Animator animator;
        private RPGPawn ownerPlayer;
        
        private static readonly int IsInAir = Animator.StringToHash("IsInAir");
        private static readonly int VelocityX = Animator.StringToHash("VelocityX");
        private static readonly int VelocityY = Animator.StringToHash("VelocityY");
        private static readonly int AttackingTrigger = Animator.StringToHash("Attacking");
        private static readonly int BreakAttackingTrigger = Animator.StringToHash("BreakAttacking");


        
        protected override void Awake()
        {
            base.Awake();
            
            OwnerChanged += () => ownerPlayer = (RPGPawn)GetOwner();
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