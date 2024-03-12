using CycloneGames.GameFramework;
using UnityEngine;

namespace ARPGSample.Gameplay
{
    public class AnimationFSM : Actor
    {
        protected static readonly int IsDead = Animator.StringToHash("IsDead");
        protected static readonly int IsInAir = Animator.StringToHash("IsInAir");
        protected static readonly int VelocityX = Animator.StringToHash("VelocityX");
        protected static readonly int VelocityY = Animator.StringToHash("VelocityY");
        protected static readonly int AttackingTrigger = Animator.StringToHash("Attacking");
        protected static readonly int BreakAttackingTrigger = Animator.StringToHash("BreakAttacking");
    }
}