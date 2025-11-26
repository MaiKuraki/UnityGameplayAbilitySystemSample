using System.Linq;
using CycloneGames.GameplayAbilities.Runtime;
using CycloneGames.GameplayFramework;
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
        private readonly bool useRootMotion;
        private readonly AnimationClip anim_camera;
        private readonly float comboWindowDuration;
        private bool cachedRootMotionState;

        public GA_Attack(AnimationClip anim_character, bool useRootMotion, AnimationClip anim_camera, GameplayEffect effect_dmg, float comboWindowDuration)
        {
            this.anim_character = anim_character;
            this.useRootMotion = useRootMotion;
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

            GASSamplePlayerController pc = (GASSamplePlayerController)character.Controller;
            GASSampleCameraManager cm = pc?.GetCameraManager() as GASSampleCameraManager;
            Animator cameraAnimator = cm.GetAnimator;

            // Immediately clean up any orphaned combo window tag to ensure a clean state.
            asc?.RemoveLooseGameplayTag(GameplayTagManager.RequestTag(GASSampleTags.Skill_State_ComboWindow));

            if (character == null)
            {
                EndAbility();
                return;

            }
            cachedRootMotionState = character.Animator.applyRootMotion;
            character.Animator.applyRootMotion = useRootMotion;
            character.Animator.CrossFade(anim_character.name, 0.1f);

            if (cameraAnimator != null && anim_camera != null)
            {
                cameraAnimator.CrossFade(anim_camera.name, 0.1f);
            }

            foreach (var tag in spec.Ability.AbilityTags)
            {
                asc?.AddLooseGameplayTag(tag);
            }

            if (this.CooldownEffectDefinition == null || this.CooldownEffectDefinition.Duration <= 0)
            {
                CLogger.LogDebug($"{DEBUG_FLAG} not have Cooldown effect.");
                character.Animator.applyRootMotion = cachedRootMotionState;
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
                character.Animator.applyRootMotion = cachedRootMotionState;
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
            GASSampleCharacter character = Spec?.Ability?.ActorInfo.OwnerActor as GASSampleCharacter;
            if (character) character.Animator.applyRootMotion = cachedRootMotionState;
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
            var ability = new GA_Attack(anim_character, useRootMotion, anim_camera, effect_dmg, comboWindowDuration);

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
        [SerializeField] private bool bUseRootMotion = true;
        [SerializeField] private AnimationClip Anim_Camera;
        [SerializeField] private float ComboWindowDuration = 0.2f;

        public override GameplayAbility CreateAbility()
        {
            var effect_dmg = DmgEffect ? DmgEffect.GetGameplayEffect() : null;
            var ability = new GA_Attack(Anim_Character, bUseRootMotion, Anim_Camera, effect_dmg, ComboWindowDuration);

            ability.Initialize(
                AbilityName,
                InstancingPolicy,
                NetExecutionPolicy,
                CostEffect?.GetGameplayEffect(),
                CooldownEffect?.GetGameplayEffect(),
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
