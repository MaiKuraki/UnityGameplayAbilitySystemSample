using System;
using System.Linq;
using CycloneGames.GameplayAbilities.Runtime;
using CycloneGames.GameplayFramework.Runtime;
using CycloneGames.GameplayTags.Core;
using CycloneGames.InputSystem.Runtime;
using GASSample.Input.Generated;
using GASSample.Message;
using GASSample.UI;
using RPGSample.Message;
using UnityEngine;

namespace GASSample.Gameplay
{
    public class GASSamplePlayerCharacter : GASSampleCharacter
    {
        [SerializeField] private Transform CameraFocusTF;
        [SerializeField] private string comboEntryAbilityTagName = GASSampleTags.Skill_Definition_Attack_1;
        [Header("Debug")]
        [SerializeField] private bool enableGasDebugLogs = false;

        private IInputPlayer inputPlayer;
        private InputContext gameplayContext;
        private Vector2 moveInput;

        protected override void Awake()
        {
            base.Awake();

            TryBindInput();
        }

        protected override void Start()
        {
            base.Start();

        }

        private void OnDestroy()
        {
            if (gameplayContext != null)
            {
                try
                {
                    inputPlayer?.RemoveContext(gameplayContext);
                }
                catch (ObjectDisposedException)
                {
                    // The input player can already be torn down when the scene unloads.
                }

                gameplayContext.Dispose();
                gameplayContext = null;
            }
        }

        /// <summary>
        /// Binds the "Gameplay" context: Attack (mouse left button / gamepad south button) and
        /// Move (WASD / left stick). The bindings live in StreamingAssets/input_config.yaml.
        /// </summary>
        private void TryBindInput()
        {
            if (gameplayContext != null)
            {
                return;
            }

            InputManager manager = InputManager.Instance;
            inputPlayer = manager != null ? manager.GetInputPlayer(0) : null;
            if (inputPlayer == null)
            {
                return;
            }

            gameplayContext = new InputContext(InputActions.ActionMaps.PlayerActions, InputActions.Contexts.Gameplay)
                .AddBinding(inputPlayer.GetButtonObservable(InputActions.Actions.Gameplay_Attack), new ActionCommand(HandleAttackInput))
                .AddBinding(inputPlayer.GetVector2Observable(InputActions.Actions.Gameplay_Move), new MoveCommand(OnMove));

            inputPlayer.PushContext(gameplayContext);
        }

        private void OnMove(Vector2 direction)
        {
            moveInput = direction;
        }

        private void Update()
        {
            if (gameplayContext == null)
            {
                // The player joins during bootstrap; retry until it exists.
                TryBindInput();
            }

            movementVelocity = ResolveWorldVelocity(moveInput);
            GetMovementComponent?.MoveWithVelocity(movementVelocity);
            AbilitySystemComponent?.Tick(Time.deltaTime, true);
        }

        /// <summary>Camera-relative move direction on the ground plane.</summary>
        private Vector3 ResolveWorldVelocity(Vector2 input)
        {
            if (input.sqrMagnitude < 0.0001f)
            {
                return Vector3.zero;
            }

            Camera referenceCamera = Camera.main;
            Vector3 forward = referenceCamera != null ? referenceCamera.transform.forward : Vector3.forward;
            Vector3 right = referenceCamera != null ? referenceCamera.transform.right : Vector3.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 direction = forward * input.y + right * input.x;
            if (direction.sqrMagnitude > 1f)
            {
                direction.Normalize();
            }

            float speed = GetMovementComponent != null ? GetMovementComponent.moveSpeed : 5f;
            return direction * speed;
        }

        private void HandleAttackInput()
        {
            if (AbilitySystemComponent == null)
            {
                return;
            }

            bool inComboWindow = AbilitySystemComponent.CombinedTags.Contains(GameplayTagManager.Request(GASSampleTags.Skill_State_ComboWindow));
            if (inComboWindow && TryActivateNextComboAbility())
            {
                return;
            }

            bool isAnyAttackActive = HasActiveComboAbility();
            if (!isAnyAttackActive)
            {
                TryActiveAbilityByTag(GameplayTagManager.Request(comboEntryAbilityTagName));
            }
        }

        private bool HasActiveComboAbility()
        {
            if (AbilitySystemComponent == null)
            {
                return false;
            }

            var abilities = AbilitySystemComponent.GetActivatableAbilities();
            foreach (var abilitySpec in abilities)
            {
                if (abilitySpec.IsActive && abilitySpec.Ability is IComboChainAbility)
                {
                    return true;
                }
            }

            return false;
        }

        private bool TryActivateNextComboAbility()
        {
            if (AbilitySystemComponent == null)
            {
                return false;
            }

            var abilities = AbilitySystemComponent.GetActivatableAbilities();
            foreach (var abilitySpec in abilities)
            {
                if (!abilitySpec.IsActive)
                {
                    continue;
                }

                if (abilitySpec.Ability is IComboChainAbility comboAbility
                    && comboAbility.TryGetNextComboAbilityTag(out GameplayTag nextTag))
                {
                    return TryActiveAbilityByTag(nextTag);
                }
            }

            return false;
        }

        protected override void NotifyControllerChanged(Controller oldController, Controller newController)
        {
            base.NotifyControllerChanged(oldController, newController);

            if (newController == null)
            {
                if (AttributeSet != null)
                {
                    AttributeSet.Health.OnCurrentValueChanged -= OnHealthChanged;
                    AttributeSet.MaxHealth.OnCurrentValueChanged -= OnHealthChanged;
                    AttributeSet.Stamina.OnCurrentValueChanged -= OnStaminaChanged;
                    AttributeSet.MaxStamina.OnCurrentValueChanged -= OnStaminaChanged;
                    AttributeSet.Experience.OnCurrentValueChanged -= OnExperienceChanged;
                }

                return;
            }

            if (AttributeSet != null)
            {
                AttributeSet.Health.OnCurrentValueChanged += OnHealthChanged;
                AttributeSet.MaxHealth.OnCurrentValueChanged += OnHealthChanged;
                AttributeSet.Stamina.OnCurrentValueChanged += OnStaminaChanged;
                AttributeSet.MaxStamina.OnCurrentValueChanged += OnStaminaChanged;
                AttributeSet.Experience.OnCurrentValueChanged += OnExperienceChanged;
            }

            ApplyInitialEffects();
            GrandInitialAbilities();

            if (newController is PlayerController pc)
            {
                var cameraManager = pc.GetCameraManager();
                if (cameraManager != null && CameraFocusTF != null)
                {
                    cameraManager.SetViewTarget(CameraFocusTF);
                }
            }
        }

        private void ApplyInitialEffects()
        {
            if (InitialAttributes != null && AbilitySystemComponent != null)
            {
                var ge = InitialAttributes.GetGameplayEffect();
                var spec = GameplayEffectSpec.Create(ge, AbilitySystemComponent);
                AbilitySystemComponent.ApplyGameplayEffectSpecToSelf(spec);
            }

            if (InitialPassiveEffects != null)
            {
                foreach (var passiveEffectSO in InitialPassiveEffects)
                {
                    if (passiveEffectSO != null)
                    {
                        var ge = passiveEffectSO.GetGameplayEffect();
                        var spec = GameplayEffectSpec.Create(ge, AbilitySystemComponent);
                        AbilitySystemComponent.ApplyGameplayEffectSpecToSelf(spec);
                    }
                }
            }

        }

        private void GrandInitialAbilities()
        {
            if (AbilitySystemComponent == null) return;
            foreach (var abilitySO in InitialAbilities)
            {
                if (abilitySO != null)
                {
                    AbilitySystemComponent.GrantAbility(abilitySO.GetGameplayAbility());
                }
            }
        }

        private bool TryActiveAbilityByTag(GameplayTag tag)
        {
            if (AbilitySystemComponent == null || tag.IsNone)
            {
                return false;
            }

            var abilities = AbilitySystemComponent.GetActivatableAbilities();
            foreach (var abilitySpec in abilities)
            {
                if (abilitySpec.Ability != null && abilitySpec.Ability.AbilityTags.HasTag(tag))
                {
                    if (enableGasDebugLogs)
                    {
                        bool isOnCooldown = AbilitySystemComponent.IsAbilityOnCooldown(abilitySpec.Ability);
                        AbilitySystemComponent.GetCooldownInfo(abilitySpec.Ability, out float cooldownRemaining, out float cooldownTotal);
                        Debug.Log($"[GASDebug] TryActivate ability={abilitySpec.Ability.Name} tag={tag} onCooldown={isOnCooldown} cooldown={cooldownRemaining:0.00}/{cooldownTotal:0.00}");
                    }

                    return AbilitySystemComponent.TryActivateAbility(abilitySpec);
                }
            }

            return false;
        }

        private void OnHealthChanged(float oldValue, float newValue)
        {
            StatusData data = new StatusData(
                AttributeSet.GetBaseValue(AttributeSet.Health),
                AttributeSet.GetCurrentValue(AttributeSet.Health),
                AttributeSet.GetBaseValue(AttributeSet.MaxHealth),
                AttributeSet.GetCurrentValue(AttributeSet.MaxHealth)
            );
            UIMessage<StatusData> msg = new UIMessage<StatusData>(MessageConstant.UpdateHealth, data);
            MessageContext.UIRouter.PublishAsync(msg);
        }

        private void OnStaminaChanged(float oldValue, float newValue)
        {
            StatusData data = new StatusData(
                AttributeSet.GetBaseValue(AttributeSet.Stamina),
                AttributeSet.GetCurrentValue(AttributeSet.Stamina),
                AttributeSet.GetBaseValue(AttributeSet.MaxStamina),
                AttributeSet.GetCurrentValue(AttributeSet.MaxStamina)
            );
            UIMessage<StatusData> msg = new UIMessage<StatusData>(MessageConstant.UpdateStamina, data);
            MessageContext.UIRouter.PublishAsync(msg);
        }

        private void OnExperienceChanged(float oldValue, float newValue)
        {
            StatusData data = new StatusData(
                AttributeSet.GetBaseValue(AttributeSet.Experience),
                AttributeSet.GetCurrentValue(AttributeSet.Experience),
                GetNextLevelExp(),
                GetNextLevelExp()
            );
            UIMessage<StatusData> msg = new UIMessage<StatusData>(MessageConstant.UpdateExperience, data);
        }

        private float GetNextLevelExp()
        {
            if (AttributeSet == null) return 0;
            int currentLevel = (int)AttributeSet.GetCurrentValue(AttributeSet.Level);
            int nextLevel = currentLevel + 1;
            return LevelUpData.Levels[nextLevel].XpToNextLevel;
        }
    }
}
