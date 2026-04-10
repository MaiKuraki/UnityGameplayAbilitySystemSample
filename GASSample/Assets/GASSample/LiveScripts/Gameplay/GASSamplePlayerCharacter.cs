using System.Linq;
using CycloneGames.GameplayAbilities.Runtime;
using CycloneGames.GameplayFramework.Runtime;
using CycloneGames.GameplayTags.Runtime;
using GASSample.Message;
using GASSample.UI;
using RPGSample.Message;
using UnityEngine;
using VContainer;

namespace GASSample.Gameplay
{
    public class GASSamplePlayerCharacter : GASSampleCharacter
    {
        [Inject] IWorld world;

        [SerializeField] private Transform CameraFocusTF;
        [SerializeField] private GameplayTag comboEntryAbilityTag = GASSampleTags.Skill_Definition_Attack_1;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

        }
        override protected void Update()
        {
            GetMovementComponent?.MoveWithVelocity(movementVelocity);
            AbilitySystemComponent?.Tick(Time.deltaTime, true);

            if (Input.GetMouseButtonDown(0))
            {
                HandleAttackInput();
            }
        }

        private void HandleAttackInput()
        {
            if (AbilitySystemComponent == null)
            {
                return;
            }

            bool inComboWindow = AbilitySystemComponent.CombinedTags.Contains(GASSampleTags.Skill_State_ComboWindow);
            if (inComboWindow && TryActivateNextComboAbility())
            {
                return;
            }

            bool isAnyAttackActive = HasActiveComboAbility();
            if (!isAnyAttackActive)
            {
                TryActiveAbilityByTag(comboEntryAbilityTag);
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

        public override void PossessedBy(Controller NewController)
        {
            base.PossessedBy(NewController);

            PlayerController pc = NewController as PlayerController;
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

            var cameraManager = pc.GetCameraManager();
            cameraManager.SetViewTarget(CameraFocusTF);
        }

        public override void UnPossessed()
        {
            if (AttributeSet != null)
            {
                AttributeSet.Health.OnCurrentValueChanged -= OnHealthChanged;
                AttributeSet.MaxHealth.OnCurrentValueChanged -= OnHealthChanged;
                AttributeSet.Stamina.OnCurrentValueChanged -= OnStaminaChanged;
                AttributeSet.MaxStamina.OnCurrentValueChanged -= OnStaminaChanged;
                AttributeSet.Experience.OnCurrentValueChanged -= OnExperienceChanged;
            }

            base.UnPossessed();
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
                    AbilitySystemComponent.GrantAbility(abilitySO.CreateAbility());
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
                    AbilitySystemComponent.CombinedTags.RemoveTag(GASSampleTags.Skill_Cooldown_Attack);
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
