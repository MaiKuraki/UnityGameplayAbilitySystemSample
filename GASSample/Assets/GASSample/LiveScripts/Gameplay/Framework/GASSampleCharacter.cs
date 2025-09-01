using System.Collections.Generic;
using CycloneGames.GameplayAbilities.Runtime;
using CycloneGames.GameplayFramework;
using CycloneGames.GameplayTags.Runtime;
using CycloneGames.Logger;
using CycloneGames.RPGFoundation;
using CycloneGames.Utility.Runtime;
using UnityEngine;

namespace GASSample.Gameplay
{
    [RequireComponent(typeof(MovementComponent))]
    public class GASSampleCharacter : Pawn
    {
        protected MovementComponent movementComponent;
        protected MovementComponent GetMovementComponent => movementComponent;
        public AbilitySystemComponent AbilitySystemComponent { get; protected set; }
        public CharacterAttributeSet AttributeSet { get; protected set; }

        [field: SerializeField] public Animator Animator { get; protected set; }

        [PropertyGroup("GAS Setup", true, GroupColorIndex = 3)]
        [SerializeField] protected GameplayEffectSO InitialAttributes;
        [SerializeField] protected List<GameplayAbilitySO> InitialAbilities;
        [SerializeField] protected List<GameplayEffectSO> InitialPassiveEffects;
        [SerializeField] protected LevelUpDataSO LevelUpData;
        [EndPropertyGroup]

        public LevelUpDataSO GetLevelUpData => LevelUpData;
        protected Vector3 movementVelocity = Vector3.zero;

        protected override void Awake()
        {
            base.Awake();

            movementComponent = GetComponent<MovementComponent>();
            var effectContextFactory = new GameplayEffectContextFactory();
            this.AbilitySystemComponent = new AbilitySystemComponent(effectContextFactory);
            AbilitySystemComponent.InitAbilityActorInfo(this, gameObject);
            AttributeSet = new CharacterAttributeSet();
            AbilitySystemComponent.AddAttributeSet(AttributeSet);
        }
        override protected void Update()
        {
            base.Update();
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
                CLogger.LogInfo($"{name} gained {levelsGained} level(s)! Reached level {finalLevel}.");

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