using CycloneGames.GameplayAbilities.Runtime;
using UnityEngine;

namespace GASSample.Gameplay
{
    public class GA_Attack : GameplayAbility
    {
        private readonly GameplayEffect attackDmgEffect;

        public GA_Attack(GameplayEffect attackDmgEffect)
        {
            this.attackDmgEffect = attackDmgEffect;
        }

        public override bool CanActivate(GameplayAbilityActorInfo actorInfo, GameplayAbilitySpec spec)
        {
            return base.CanActivate(actorInfo, spec);
        }

        public override void ActivateAbility(GameplayAbilityActorInfo actorInfo, GameplayAbilitySpec spec, GameplayAbilityActivationInfo activationInfo)
        {
            // base.ActivateAbility(actorInfo, spec, activationInfo);
            CommitAbility(actorInfo, spec);

            EndAbility();
        }

        public override GameplayAbility CreatePoolableInstance()
        {
            var ability = new GA_Attack(attackDmgEffect);

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
        public GameplayEffectSO DmgEffect;
        public override GameplayAbility CreateAbility()
        {
            var effect_dmg = DmgEffect ? DmgEffect.CreateGameplayEffect() : null;
            var ability = new GA_Attack(effect_dmg);

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