using System;
using CycloneGames.GameplayAbilities.Runtime;
using CycloneGames.GameplayFramework;
using CycloneGames.GameplayFramework.Runtime;
using CycloneGames.GameplayTags.Runtime;
using CycloneGames.Logger;
using CycloneGames.Utility.Runtime;
using UnityEngine;
using UnityEngine.Serialization;

namespace GASSample.Gameplay
{
    public interface IComboChainAbility
    {
        bool TryGetNextComboAbilityTag(out GameplayTag tag);
    }

    public class GA_Attack : GameplayAbility, IComboChainAbility
    {
        private readonly AnimationClip animCharacter;
        private readonly bool useRootMotion;
        private readonly AbilityCameraClip animCamera;
        private readonly GameplayEffect effectDamage;
        private readonly float comboWindowDuration;
        private readonly AttackCameraRuntimePolicy cameraPolicy;
        private readonly GameplayTag nextComboAbilityTag;

        private readonly AbilityCinematicMode cameraModeInstance = new AbilityCinematicMode();
        private bool cachedRootMotionState;
        private Animator runtimeAnimator;
        private int activationSerial;
        private bool isSkillCameraActive;
        private bool comboWindowTagApplied;
        private bool nextComboTagApplied;
        private bool runtimeStateCleaned;

        public GA_Attack(
            AnimationClip animCharacter,
            bool useRootMotion,
            AbilityCameraClip animCamera,
            GameplayEffect effectDamage,
            float comboWindowDuration,
            AttackCameraRuntimePolicy cameraPolicy,
            GameplayTag nextComboAbilityTag)
        {
            this.animCharacter = animCharacter;
            this.useRootMotion = useRootMotion;
            this.animCamera = animCamera;
            this.effectDamage = effectDamage;
            this.comboWindowDuration = comboWindowDuration;
            this.cameraPolicy = cameraPolicy;
            this.nextComboAbilityTag = nextComboAbilityTag;
        }

        public bool TryGetNextComboAbilityTag(out GameplayTag tag)
        {
            tag = nextComboAbilityTag;
            return !tag.IsNone;
        }

        public override void ActivateAbility(GameplayAbilityActorInfo actorInfo, GameplayAbilitySpec spec, GameplayAbilityActivationInfo activationInfo)
        {
            activationSerial++;
            runtimeStateCleaned = false;
            comboWindowTagApplied = false;
            nextComboTagApplied = false;

            GASSampleCharacter character = actorInfo.OwnerActor as GASSampleCharacter;
            if (character == null)
            {
                EndAbility();
                return;
            }

            CommitAbility(actorInfo, spec);

            if (character.Animator != null && animCharacter != null)
            {
                cachedRootMotionState = character.Animator.applyRootMotion;
                runtimeAnimator = character.Animator;
                character.Animator.applyRootMotion = useRootMotion;
                character.Animator.CrossFade(animCharacter.name, 0.1f);
            }
            else
            {
                runtimeAnimator = null;
            }

            int serial = activationSerial;
            var playerController = ResolvePlayerController(actorInfo.AvatarActor);
            float animationDuration = animCharacter != null ? animCharacter.length : 0f;

            StartSkillCamera(playerController);

            BeginComboWindow(serial, animationDuration);

            if (cameraPolicy.enableCamera
                && animCamera != null
                && (cameraPolicy.stopPolicy == CameraStopPolicy.ByCameraFinished
                    || cameraPolicy.stopPolicy == CameraStopPolicy.FirstReached))
            {
                BeginCameraAutoStop(serial, playerController);
            }

            float activeDuration = ResolveActiveDuration(animationDuration);
            if (activeDuration <= 0f)
            {
                CleanupRuntimeState(serial, playerController);
                EndAbility();
                return;
            }

            var durationTask = AbilityTask_WaitDelay.WaitDelay(this, activeDuration);
            durationTask.OnFinishDelay += () =>
            {
                if (durationTask.IsCancelled || serial != activationSerial)
                {
                    return;
                }

                if (cameraPolicy.stopPolicy == CameraStopPolicy.ByAbilityEnd
                    || cameraPolicy.stopPolicy == CameraStopPolicy.FirstReached)
                {
                    StopSkillCamera(playerController);
                }

                CleanupRuntimeState(serial, playerController);
                EndAbility();
            };
            durationTask.Activate();
        }

        public override void CancelAbility()
        {
            CleanupRuntimeState(activationSerial, ResolvePlayerController(ActorInfo.AvatarActor));
            base.CancelAbility();
        }

        public override void OnRemoveAbility()
        {
            CleanupRuntimeState(activationSerial, ResolvePlayerController(ActorInfo.AvatarActor));
            base.OnRemoveAbility();
        }

        public override GameplayAbility CreatePoolableInstance()
        {
            var ability = new GA_Attack(
                animCharacter,
                useRootMotion,
                animCamera,
                effectDamage,
                comboWindowDuration,
                cameraPolicy,
                nextComboAbilityTag);

            ability.Initialize(
                Name,
                InstancingPolicy,
                NetExecutionPolicy,
                CostEffectDefinition,
                CooldownEffectDefinition,
                AbilityTags,
                ActivationBlockedTags,
                ActivationRequiredTags,
                CancelAbilitiesWithTag,
                BlockAbilitiesWithTag,
                ActivationOwnedTags,
                ActivateAbilityOnGranted,
                SourceRequiredTags,
                SourceBlockedTags,
                TargetRequiredTags,
                TargetBlockedTags,
                AbilityTriggers as System.Collections.Generic.List<AbilityTriggerData>);

            return ability;
        }

        private void StartSkillCamera(PlayerController playerController)
        {
            if (!cameraPolicy.enableCamera || isSkillCameraActive || playerController == null || animCamera == null)
            {
                return;
            }

            while (playerController.RemoveCameraMode(cameraModeInstance))
            {
            }

            float blendOutDuration = cameraPolicy.blendOutDuration > 0f ? cameraPolicy.blendOutDuration : 0.15f;
            cameraModeInstance.Setup(animCamera, blendOutDuration);
            if (playerController.TryPushCameraMode(cameraModeInstance))
            {
                isSkillCameraActive = true;
            }
        }

        private void StopSkillCamera(PlayerController playerController)
        {
            if (!isSkillCameraActive)
            {
                return;
            }

            if (playerController == null)
            {
                isSkillCameraActive = false;
                return;
            }

            bool removedAny = false;
            while (playerController.RemoveCameraMode(cameraModeInstance))
            {
                removedAny = true;
            }

            if (removedAny)
            {
                isSkillCameraActive = false;
            }
        }

        private void BeginCameraAutoStop(int serial, PlayerController playerController)
        {
            float delay = Mathf.Max(0f, GetCameraClipDuration());
            var waitTask = AbilityTask_WaitDelay.WaitDelay(this, delay);
            waitTask.OnFinishDelay += () =>
            {
                if (waitTask.IsCancelled || serial != activationSerial)
                {
                    return;
                }

                PollCameraStop(serial, playerController, 0f);
            };
            waitTask.Activate();
        }

        private void PollCameraStop(int serial, PlayerController playerController, float elapsed)
        {
            if (serial != activationSerial || !isSkillCameraActive)
            {
                return;
            }

            bool finished = cameraModeInstance.IsFinished;
            bool settled = !cameraPolicy.requireSettledBeforeRemove || cameraModeInstance.IsSettled(
                cameraPolicy.settledPositionEpsilon,
                cameraPolicy.settledFovEpsilon,
                cameraPolicy.defaultFov > 0f ? cameraPolicy.defaultFov : 60f);

            if (finished && settled)
            {
                StopSkillCamera(playerController);
                return;
            }

            float timeout = Mathf.Max(0.01f, cameraPolicy.stopSettleTimeout);
            if (elapsed >= timeout)
            {
                StopSkillCamera(playerController);
                return;
            }

            float interval = Mathf.Max(0.01f, cameraPolicy.stopCheckInterval);
            var waitTask = AbilityTask_WaitDelay.WaitDelay(this, interval);
            waitTask.OnFinishDelay += () =>
            {
                if (waitTask.IsCancelled || serial != activationSerial)
                {
                    return;
                }

                PollCameraStop(serial, playerController, elapsed + interval);
            };
            waitTask.Activate();
        }

        private void CleanupRuntimeState(int serial, PlayerController playerController)
        {
            if (runtimeStateCleaned || serial != activationSerial)
            {
                return;
            }

            runtimeStateCleaned = true;
            CloseComboWindow();
            StopSkillCamera(playerController);

            if (runtimeAnimator != null)
            {
                runtimeAnimator.applyRootMotion = cachedRootMotionState;
                runtimeAnimator = null;
            }
        }

        private void BeginComboWindow(int serial, float animationDuration)
        {
            if (comboWindowDuration <= 0f)
            {
                return;
            }

            float delay = Mathf.Max(0f, animationDuration - comboWindowDuration);
            var waitTask = AbilityTask_WaitDelay.WaitDelay(this, delay);
            waitTask.OnFinishDelay += () =>
            {
                if (waitTask.IsCancelled || serial != activationSerial)
                {
                    return;
                }

                OpenComboWindow();
            };
            waitTask.Activate();
        }

        private void OpenComboWindow()
        {
            if (comboWindowTagApplied || AbilitySystemComponent == null)
            {
                return;
            }

            AbilitySystemComponent.CombinedTags.AddTag(GASSampleTags.Skill_State_ComboWindow);
            comboWindowTagApplied = true;

            if (!nextComboAbilityTag.IsNone)
            {
                AbilitySystemComponent.CombinedTags.AddTag(nextComboAbilityTag);
                nextComboTagApplied = true;
            }
        }

        private void CloseComboWindow()
        {
            if (!comboWindowTagApplied || AbilitySystemComponent == null)
            {
                return;
            }

            AbilitySystemComponent.CombinedTags.RemoveTag(GASSampleTags.Skill_State_ComboWindow);
            comboWindowTagApplied = false;

            if (nextComboTagApplied)
            {
                AbilitySystemComponent.CombinedTags.RemoveTag(nextComboAbilityTag);
                nextComboTagApplied = false;
            }
        }

        private float ResolveActiveDuration(float animationDuration)
        {
            return Mathf.Max(0f, animationDuration);
        }

        private float GetCameraClipDuration()
        {
            return animCamera != null ? animCamera.GetTotalDuration() : 0f;
        }

        private static PlayerController ResolvePlayerController(object avatarActor)
        {
            if (avatarActor is PlayerController pc)
            {
                return pc;
            }

            if (avatarActor is Pawn pawn)
            {
                return pawn.Controller as PlayerController;
            }

            if (avatarActor is Component component)
            {
                var pawnComponent = component.GetComponent<Pawn>();
                if (pawnComponent != null)
                {
                    return pawnComponent.Controller as PlayerController;
                }

                return component.GetComponent<PlayerController>();
            }

            if (avatarActor is GameObject go)
            {
                var pawnFromGo = go.GetComponent<Pawn>();
                if (pawnFromGo != null)
                {
                    return pawnFromGo.Controller as PlayerController;
                }

                return go.GetComponent<PlayerController>();
            }

            return null;
        }
    }

    [CreateAssetMenu(fileName = "GA_Attack", menuName = "GASSample/Gameplay/GAS/GA/Attack")]
    public class GA_Attack_SO : GameplayAbilitySO
    {
        [Header("Attack")]
        [FormerlySerializedAs("Anim_Character")]
        [SerializeField] private AnimationClip animCharacter;
        [SerializeField] private bool useRootMotion = true;
        [FormerlySerializedAs("DmgEffect")]
        [SerializeField] private GameplayEffectSO damageEffect;
        [FormerlySerializedAs("ComboWindowDuration")]
        [SerializeField, Min(0f)] private float comboWindowDuration = 0.5f;
        [FormerlySerializedAs("NextComboAbility")]
        [SerializeField] private GameplayAbilitySO nextComboAbility;

        [Header("Camera")]
        [FormerlySerializedAs("Anim_Camera")]
        [SerializeField] private AbilityCameraClip cameraClip;
        [SerializeField] private AttackCameraRuntimePolicySO cameraPolicy;

        public override GameplayAbility CreateAbility()
        {
            var runtimePolicy = cameraPolicy != null ? cameraPolicy.Value : AttackCameraRuntimePolicy.Default;
            var runtimeDamageEffect = damageEffect ? damageEffect.GetGameplayEffect() : null;
            var nextComboTag = ResolveFirstAbilityTag(nextComboAbility);

            var ability = new GA_Attack(
                animCharacter,
                useRootMotion,
                cameraClip,
                runtimeDamageEffect,
                comboWindowDuration,
                runtimePolicy,
                nextComboTag);

            InitializeAbility(ability);

            return ability;
        }

        private static GameplayTag ResolveFirstAbilityTag(GameplayAbilitySO abilitySO)
        {
            if (abilitySO == null || abilitySO.AbilityTags == null || abilitySO.AbilityTags.IsEmpty)
            {
                return default;
            }

            foreach (var tag in abilitySO.AbilityTags.GetExplicitTags())
            {
                return tag;
            }

            return default;
        }
    }
}