using System;
using System.Linq;
using CycloneGames.GameplayAbilities.Runtime;
using CycloneGames.GameplayTags.Core;
using CycloneGames.InputSystem.Runtime;
using GASSample.Input.Generated;
using UnityEngine;

namespace GASSample.Gameplay
{
    /// <summary>
    /// Player-controlled character: binds input to the ability system and exposes the attributes the
    /// HUD listens to.
    /// </summary>
    public class GASSamplePlayerCharacter : GASSampleCharacter
    {
        [SerializeField] private string comboEntryAbilityTagName = GASSampleTags.Skill_Definition_Attack_1;

        [Tooltip("Degrees per second the character turns with horizontal input.")]
        [SerializeField] private float turnSpeed = 180f;

        [Header("Debug")]
        [SerializeField] private bool enableGasDebugLogs = false;

        /// <summary>Raised as (current, max).</summary>
        public event Action<float, float> HealthChanged;
        public event Action<float, float> StaminaChanged;
        public event Action<float, float> ExperienceChanged;

        public float Health => AttributeSet.GetCurrentValue(AttributeSet.Health);
        public float MaxHealth => AttributeSet.GetCurrentValue(AttributeSet.MaxHealth);
        public float Stamina => AttributeSet.GetCurrentValue(AttributeSet.Stamina);
        public float MaxStamina => AttributeSet.GetCurrentValue(AttributeSet.MaxStamina);
        public float Experience => AttributeSet.GetCurrentValue(AttributeSet.Experience);
        public float ExperienceToNextLevel => GetNextLevelExp();

        private IInputPlayer inputPlayer;
        private InputContext gameplayContext;
        private Vector2 moveInput;

        protected override void Awake()
        {
            base.Awake();

            TryBindInput();
        }

        protected override void OnDestroy()
        {
            if (gameplayContext != null)
            {
                inputPlayer?.RemoveContext(gameplayContext);
                gameplayContext.Dispose();
                gameplayContext = null;
            }

            base.OnDestroy();
        }

        /// <summary>
        /// Binds the "Gameplay" context: Attack and Move, as declared in StreamingAssets/input_config.yaml.
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
                // The input player is created during bootstrap; retry until it exists.
                TryBindInput();
            }

            transform.Rotate(0f, moveInput.x * turnSpeed * Time.deltaTime, 0f, Space.World);

            movementVelocity = transform.forward * (moveInput.y * GetMovementComponent.moveSpeed);
            GetMovementComponent.MoveWithVelocity(movementVelocity);
            AbilitySystemComponent?.Tick(Time.deltaTime, true);
        }

        private void HandleAttackInput()
        {
            bool inComboWindow = AbilitySystemComponent.CombinedTags.Contains(GameplayTagManager.Request(GASSampleTags.Skill_State_ComboWindow));
            if (inComboWindow && TryActivateNextComboAbility())
            {
                return;
            }

            if (!HasActiveComboAbility())
            {
                TryActivateAbilityByTag(GameplayTagManager.Request(comboEntryAbilityTagName));
            }
        }

        private bool HasActiveComboAbility()
        {
            return AbilitySystemComponent.GetActivatableAbilities()
                .Any(spec => spec.IsActive && spec.Ability is IComboChainAbility);
        }

        private bool TryActivateNextComboAbility()
        {
            foreach (GameplayAbilitySpec spec in AbilitySystemComponent.GetActivatableAbilities())
            {
                if (spec.IsActive
                    && spec.Ability is IComboChainAbility comboAbility
                    && comboAbility.TryGetNextComboAbilityTag(out GameplayTag nextTag))
                {
                    return TryActivateAbilityByTag(nextTag);
                }
            }

            return false;
        }

        private bool TryActivateAbilityByTag(GameplayTag tag)
        {
            foreach (GameplayAbilitySpec spec in AbilitySystemComponent.GetActivatableAbilities())
            {
                if (spec.Ability == null || !spec.Ability.AbilityTags.HasTag(tag))
                {
                    continue;
                }

                if (enableGasDebugLogs)
                {
                    AbilitySystemComponent.GetCooldownInfo(spec.Ability, out float remaining, out float total);
                    Debug.Log($"[GASDebug] {spec.Ability.Name} tag={tag} cooldown={remaining:0.00}/{total:0.00}");
                }

                return AbilitySystemComponent.TryActivateAbility(spec);
            }

            return false;
        }

        protected override void OnHealthChanged(float oldValue, float newValue)
        {
            HealthChanged?.Invoke(Health, MaxHealth);
        }

        protected override void OnStaminaChanged(float oldValue, float newValue)
        {
            StaminaChanged?.Invoke(Stamina, MaxStamina);
        }

        protected override void OnExperienceChanged(float oldValue, float newValue)
        {
            ExperienceChanged?.Invoke(Experience, ExperienceToNextLevel);
        }

        private float GetNextLevelExp()
        {
            int currentLevel = (int)AttributeSet.GetCurrentValue(AttributeSet.Level);
            return LevelUpData.Levels[currentLevel + 1].XpToNextLevel;
        }
    }
}
