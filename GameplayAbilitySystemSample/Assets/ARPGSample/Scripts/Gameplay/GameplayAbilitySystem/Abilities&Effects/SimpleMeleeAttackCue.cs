using CycloneGames.GameFramework;
using UnityEngine;

namespace ARPGSample.Gameplay
{
    public class SimpleMeleeAttackCue : Actor
    {
        [SerializeField] private Animator HitAnimator;
        private static readonly int Hit = Animator.StringToHash("Hit");


        protected override void Awake()
        {
            base.Awake();

            TriggerHit();
        }

        public void TriggerHit()
        {
            HitAnimator.SetTrigger(Hit);
        }
    }
}
