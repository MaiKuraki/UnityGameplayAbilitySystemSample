using System.Collections.Generic;
using CycloneGames.GameplayAbilities.Runtime;
using CycloneGames.GameplayTags.Core;
using CycloneGames.Logging;
using CycloneGames.RPGFoundation;
using CycloneGames.Utility.Runtime;
using UnityEngine;

namespace GASSample.Gameplay
{
    /// <summary>
    /// Anything that owns an ability system: builds the component, wires it to this GameObject as the
    /// avatar, and applies the effects and abilities authored on the prefab.
    /// </summary>
    [RequireComponent(typeof(MovementComponent))]
    public class GASSampleCharacter : MonoBehaviour
    {
        private static readonly LogChannel Log = LogChannel.Create("GASSample");

        [field: SerializeField] public Animator Animator { get; protected set; }

        [SerializeField] private Transform cameraFocus;

        [PropertyGroup("GAS Setup", true, GroupColorIndex = 3)]
        [SerializeField] protected GameplayEffectSO InitialAttributes;
        [SerializeField] protected List<GameplayAbilitySO> InitialAbilities;
        [SerializeField] protected List<GameplayEffectSO> InitialPassiveEffects;
        [SerializeField] protected LevelUpDataSO LevelUpData;
        [EndPropertyGroup]

        public AbilitySystemComponent AbilitySystemComponent { get; protected set; }
        public CharacterAttributeSet AttributeSet { get; protected set; }
        public IAbilityCamera Camera { get; private set; }
        public Transform CameraFocus => cameraFocus;
        public LevelUpDataSO GetLevelUpData => LevelUpData;

        protected MovementComponent movementComponent;
        protected Vector3 movementVelocity = Vector3.zero;

        protected MovementComponent GetMovementComponent => movementComponent;

        /// <summary>Called by the spawner once the scene camera exists.</summary>
        public void SetCamera(IAbilityCamera camera)
        {
            Camera = camera;
        }

        protected virtual void Awake()
        {
            movementComponent = GetComponent<MovementComponent>();

            AbilitySystemComponent = new AbilitySystemComponent(
                runtimeContext: null,
                effectContextFactory: new GameplayEffectContextFactory());
            AbilitySystemComponent.InitAbilityActorInfo(this, gameObject);

            AttributeSet = new CharacterAttributeSet();
            AbilitySystemComponent.AddAttributeSet(AttributeSet);
        }

        protected virtual void Start()
        {
            AttributeSet.Health.OnCurrentValueChanged += OnHealthChanged;
            AttributeSet.MaxHealth.OnCurrentValueChanged += OnHealthChanged;
            AttributeSet.Stamina.OnCurrentValueChanged += OnStaminaChanged;
            AttributeSet.MaxStamina.OnCurrentValueChanged += OnStaminaChanged;
            AttributeSet.Experience.OnCurrentValueChanged += OnExperienceChanged;

            ApplyEffect(InitialAttributes);

            if (InitialPassiveEffects != null)
            {
                foreach (GameplayEffectSO passiveEffect in InitialPassiveEffects)
                {
                    ApplyEffect(passiveEffect);
                }
            }

            if (InitialAbilities != null)
            {
                foreach (GameplayAbilitySO abilityAsset in InitialAbilities)
                {
                    if (abilityAsset != null)
                    {
                        AbilitySystemComponent.GrantAbility(abilityAsset.GetGameplayAbility());
                    }
                }
            }
        }

        protected virtual void OnDestroy()
        {
            AbilitySystemComponent?.Dispose();
        }

        protected virtual void OnHealthChanged(float oldValue, float newValue)
        {
        }

        protected virtual void OnStaminaChanged(float oldValue, float newValue)
        {
        }

        protected virtual void OnExperienceChanged(float oldValue, float newValue)
        {
        }

        private void ApplyEffect(GameplayEffectSO effectAsset)
        {
            if (effectAsset == null)
            {
                return;
            }

            GameplayEffectSpec spec = GameplayEffectSpec.Create(effectAsset.GetGameplayEffect(), AbilitySystemComponent);
            AbilitySystemComponent.ApplyGameplayEffectSpecToSelf(spec);
        }

        public void CheckForLevelUp()
        {
            if (LevelUpData == null) return;

            int initialLevel = (int)AttributeSet.GetCurrentValue(AttributeSet.Level);
            int currentXP = (int)AttributeSet.GetCurrentValue(AttributeSet.Experience);

            int levelsGained = 0;
            int xpCostTotal = 0;

            float healthGain = 0;
            float staminaGain = 0;
            float attackGain = 0;
            float defenseGain = 0;

            int tempLevelTracker = initialLevel;

            while (tempLevelTracker < LevelUpData.Levels.Count && currentXP >= LevelUpData.Levels[tempLevelTracker - 1].XpToNextLevel)
            {
                LevelData levelData = LevelUpData.Levels[tempLevelTracker - 1];

                currentXP -= levelData.XpToNextLevel;
                xpCostTotal += levelData.XpToNextLevel;

                levelsGained++;
                tempLevelTracker++;

                healthGain += levelData.HealthGain;
                staminaGain += levelData.StaminaGain;
                attackGain += levelData.AttackGain;
                defenseGain += levelData.DefenseGain;
            }

            if (levelsGained > 0)
            {
                int finalLevel = initialLevel + levelsGained;
                Log.Info($"{name} gained {levelsGained} level(s)! Reached level {finalLevel}.");

                var mods = new List<ModifierInfo>
                {
                    new ModifierInfo(AttributeSet.Experience, EAttributeModifierOperation.Add, -xpCostTotal),
                    new ModifierInfo(AttributeSet.Level, EAttributeModifierOperation.Add, levelsGained),
                    new ModifierInfo(AttributeSet.MaxHealth, EAttributeModifierOperation.Add, healthGain),
                    new ModifierInfo(AttributeSet.Health, EAttributeModifierOperation.Add, healthGain),
                    new ModifierInfo(AttributeSet.MaxStamina, EAttributeModifierOperation.Add, staminaGain),
                    new ModifierInfo(AttributeSet.Stamina, EAttributeModifierOperation.Add, staminaGain),
                    new ModifierInfo(AttributeSet.AttackPower, EAttributeModifierOperation.Add, attackGain),
                    new ModifierInfo(AttributeSet.Defense, EAttributeModifierOperation.Add, defenseGain)
                };

                var levelUpEffect = new GameplayEffect($"GE_MultiLevelUp_ToLvl{finalLevel}", EDurationPolicy.Instant, 0, 0, mods);

                var spec = GameplayEffectSpec.Create(levelUpEffect, AbilitySystemComponent);
                AbilitySystemComponent.ApplyGameplayEffectSpecToSelf(spec);
            }
        }
    }
}
