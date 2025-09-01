using CycloneGames.GameplayAbilities.Runtime;
using CycloneGames.Utility.Runtime;
using UnityEngine;

namespace GASSample.Gameplay
{
    public class GA_Attack : GameplayAbility
    {
        private readonly GameplayEffect effect_dmg;
        private readonly AnimationClip anim_character;
        private readonly AnimationClip anim_camera;

        public GA_Attack(AnimationClip anim_character, AnimationClip anim_camera, GameplayEffect effect_dmg)
        {
            this.anim_character = anim_character;
            this.anim_camera = anim_camera;
            this.effect_dmg = effect_dmg;
        }

        public override bool CanActivate(GameplayAbilityActorInfo actorInfo, GameplayAbilitySpec spec)
        {
            return base.CanActivate(actorInfo, spec);
        }

        public override void ActivateAbility(GameplayAbilityActorInfo actorInfo, GameplayAbilitySpec spec, GameplayAbilityActivationInfo activationInfo)
        {
            // base.ActivateAbility(actorInfo, spec, activationInfo);
            CommitAbility(actorInfo, spec);

            GASSampleCharacter character = actorInfo.OwnerActor as GASSampleCharacter;
            character.Animator.CrossFade(anim_character.name, 0.1f);
            
            EndAbility();
        }

        public override GameplayAbility CreatePoolableInstance()
        {
            var ability = new GA_Attack(anim_character, anim_camera, effect_dmg);

            ability.Initialize(
                this.Name,
                this.InstancingPolicy,
                this.NetExecutionPolicy,
                this.CostEffectDefinition,
                this.CooldownEffectDefinition,
                this.AbilityTags,
                this.ActivationBlockedTags,
                this.ActivationRequiredTags,
                this.CancelAbilitiesWithTag,
                this.BlockAbilitiesWithTag
            );

            return ability;
        }
    }

    [CreateAssetMenu(fileName = "GA_Attack", menuName = "GASSample/Gameplay/GAS/GA/Attack")]
    public class GA_Attack_SO : GameplayAbilitySO
    {
        [PropertyGroup("Additional Config", true)]
        [SerializeField] private GameplayEffectSO DmgEffect;
        [SerializeField] private AnimationClip Anim_Character;
        [SerializeField] private AnimationClip Anim_Camera;

        public override GameplayAbility CreateAbility()
        {
            var effect_dmg = DmgEffect ? DmgEffect.CreateGameplayEffect() : null;
            var ability = new GA_Attack(Anim_Character, Anim_Camera, effect_dmg);

            ability.Initialize(
                AbilityName,
                InstancingPolicy,
                NetExecutionPolicy,
                CostEffect?.CreateGameplayEffect(),
                CooldownEffect?.CreateGameplayEffect(),
                AbilityTags,
                ActivationBlockedTags,
                ActivationRequiredTags,
                CancelAbilitiesWithTag,
                BlockAbilitiesWithTag
            );

            return ability;
        }
    }
}
