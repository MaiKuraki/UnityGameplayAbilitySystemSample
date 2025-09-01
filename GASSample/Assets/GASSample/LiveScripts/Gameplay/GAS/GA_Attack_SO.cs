using System.Linq;
using CycloneGames.GameplayAbilities.Runtime;
using CycloneGames.GameplayTags.Runtime;
using CycloneGames.Logger;
using CycloneGames.Utility.Runtime;
using UnityEngine;

namespace GASSample.Gameplay
{
    public class GA_Attack : GameplayAbility
    {
        private const string DEBUG_FLAG = "[GameplayAbility]";
        private readonly GameplayEffect effect_dmg;
        private readonly AnimationClip anim_character;
        private readonly AnimationClip anim_camera;
        private readonly float comboWindowDuration;

        public GA_Attack(AnimationClip anim_character, AnimationClip anim_camera, GameplayEffect effect_dmg, float comboWindowDuration)
        {
            this.anim_character = anim_character;
            this.anim_camera = anim_camera;
            this.effect_dmg = effect_dmg;
            this.comboWindowDuration = comboWindowDuration;
        }

        public override bool CanActivate(GameplayAbilityActorInfo actorInfo, GameplayAbilitySpec spec)
        {
            return base.CanActivate(actorInfo, spec);
        }

        public override void ActivateAbility(GameplayAbilityActorInfo actorInfo, GameplayAbilitySpec spec, GameplayAbilityActivationInfo activationInfo)
        {
            CommitAbility(actorInfo, spec);

            GASSampleCharacter character = actorInfo.OwnerActor as GASSampleCharacter;
            AbilitySystemComponent asc = character?.AbilitySystemComponent;

            // Immediately clean up any orphaned combo window tag to ensure a clean state.
            asc?.RemoveLooseGameplayTag(GameplayTagManager.RequestTag(GASSampleTags.Skill_State_ComboWindow));

            if (character == null)
            {
                EndAbility();
                return;
            }
            character.Animator.CrossFade(anim_character.name, 0.1f);
            foreach (var tag in spec.Ability.AbilityTags)
            {
                asc?.AddLooseGameplayTag(tag);
            }

            if (this.CooldownEffectDefinition == null || this.CooldownEffectDefinition.Duration <= 0)
            {
                CLogger.LogDebug($"{DEBUG_FLAG} not have Cooldown effect.");
                EndAbility();
                return;
            }

            float cooldownDuration = this.CooldownEffectDefinition.Duration;

            var comboWindowStartTask = AbilityTask_WaitDelay.WaitDelay(this, cooldownDuration - comboWindowDuration);
            comboWindowStartTask.OnFinishDelay += () =>
            {
                if (!comboWindowStartTask.IsCancelled)
                {
                    ComboWindowStart(asc);
                }
            };
            comboWindowStartTask.Activate();

            var comboWindowEndTask = AbilityTask_WaitDelay.WaitDelay(this, cooldownDuration);
            comboWindowEndTask.OnFinishDelay += () =>
            {
                if (!comboWindowEndTask.IsCancelled)
                {
                    ComboWindowEnd(asc);
                }
                foreach (var tag in spec.Ability.AbilityTags)
                {
                    asc?.RemoveLooseGameplayTag(tag);
                }
                EndAbility();
            };
            comboWindowEndTask.Activate();
        }

        public override void CancelAbility()
        {
            CLogger.LogInfo($"{DEBUG_FLAG} CancelAbility");
            var asc = this.AbilitySystemComponent;
            if (asc != null && this.Spec != null)
            {
                var definitionTag = this.Spec.Ability.AbilityTags.GetTags().FirstOrDefault();
                if (definitionTag != null)
                {
                    asc.RemoveLooseGameplayTag(definitionTag);
                }
            }
            base.CancelAbility();
        }

        void ComboWindowStart(AbilitySystemComponent asc)
        {
            CLogger.LogInfo($"Combo Window Start");
            asc.AddLooseGameplayTag(GameplayTagManager.RequestTag(GASSampleTags.Skill_State_ComboWindow));
        }

        void ComboWindowEnd(AbilitySystemComponent asc)
        {
            CLogger.LogInfo($"Combo Window End");
            asc.RemoveLooseGameplayTag(GameplayTagManager.RequestTag(GASSampleTags.Skill_State_ComboWindow));
        }

        public override GameplayAbility CreatePoolableInstance()
        {
            var ability = new GA_Attack(anim_character, anim_camera, effect_dmg, comboWindowDuration);

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
        [SerializeField] private float ComboWindowDuration = 0.2f;

        public override GameplayAbility CreateAbility()
        {
            var effect_dmg = DmgEffect ? DmgEffect.CreateGameplayEffect() : null;
            var ability = new GA_Attack(Anim_Character, Anim_Camera, effect_dmg, ComboWindowDuration);

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
