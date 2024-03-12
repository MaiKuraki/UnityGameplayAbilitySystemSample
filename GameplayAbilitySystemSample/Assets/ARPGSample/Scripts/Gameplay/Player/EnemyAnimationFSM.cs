using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ARPGSample.Gameplay
{
    public class EnemyAnimationFSM : AnimationFSM
    {
        [SerializeField] private Animator animator;
        
        protected static readonly int HitTrigger = Animator.StringToHash("Hit");

        private AICharacter aiCharacter;

        protected override void Awake()
        {
            base.Awake();

            aiCharacter = GetComponent<AICharacter>();
            SyncParam().Forget();
        }

        public void TriggerHit()
        {
            if(aiCharacter && animator) animator.SetTrigger(HitTrigger);
        }

        public void Dead()
        {
            animator.SetBool(IsDead, true);
        }
        
        async UniTask SyncParam()
        {
            while (true)
            {
                if (aiCharacter)
                {
                    animator.SetBool(IsInAir, aiCharacter.IsInAir);
                    
                    if (aiCharacter.RB)
                    {
                        animator.SetFloat(VelocityY, aiCharacter.RB.velocity.y);
                        animator.SetFloat(VelocityX, Mathf.Abs(aiCharacter.RB.velocity.x));
                    }
                }
                await UniTask.DelayFrame(1);
            }
        }
    }
}

