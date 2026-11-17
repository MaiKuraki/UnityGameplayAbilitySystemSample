using CycloneGames.GameplayAbilities.Core;
using CycloneGames.GameplayAbilities.Runtime;
using CycloneGames.GameplayTags.Core;
using CycloneGames.Logging;

namespace GASSample.Gameplay
{
    public class CharacterAttributeSet : AttributeSet
    {
        private static readonly LogChannel Log = LogChannel.Create("GASSample");
        public GameplayAttribute Level { get; } = new GameplayAttribute(GASSampleTags.Attribute_Primary_Level);

        public GameplayAttribute Experience { get; } = new GameplayAttribute(GASSampleTags.Attribute_Meta_Experience);
        public GameplayAttribute AttackPower { get; } = new GameplayAttribute(GASSampleTags.Attribute_Primary_Attack);
        public GameplayAttribute Defense { get; } = new GameplayAttribute(GASSampleTags.Attribute_Primary_Defense);

        public GameplayAttribute MoveSpeed { get; } = new GameplayAttribute(GASSampleTags.Attribute_Secondary_MoveSpeed);
        public GameplayAttribute Health { get; } = new GameplayAttribute(GASSampleTags.Attribute_Secondary_Health);
        public GameplayAttribute MaxHealth { get; } = new GameplayAttribute(GASSampleTags.Attribute_Secondary_MaxHealth);
        public GameplayAttribute Stamina { get; } = new GameplayAttribute(GASSampleTags.Attribute_Secondary_Stamina);
        public GameplayAttribute MaxStamina { get; } = new GameplayAttribute(GASSampleTags.Attribute_Secondary_MaxStamina);

        public GameplayAttribute Damage { get; } = new GameplayAttribute(GASSampleTags.Attribute_Meta_Damage);

        public CharacterAttributeSet() { }

        protected override void RegisterAttributes()
        {
            RegisterAttribute(Level);
            RegisterAttribute(Experience);
            RegisterAttribute(AttackPower);
            RegisterAttribute(Defense);
            RegisterAttribute(MoveSpeed);
            RegisterAttribute(Health);
            RegisterAttribute(MaxHealth);
            RegisterAttribute(Stamina);
            RegisterAttribute(MaxStamina);
            RegisterAttribute(Damage);
        }

        public override void PreAttributeChange(GameplayAttribute attribute, ref GASFixedValue newValue)
        {
            base.PreAttributeChange(attribute, ref newValue);

            if (attribute == Health)
            {
                ClampToRange(ref newValue, MaxHealth);
            }
            else if (attribute == Stamina)
            {
                ClampToRange(ref newValue, MaxStamina);
            }
        }

        private void ClampToRange(ref GASFixedValue value, GameplayAttribute maxAttribute)
        {
            GASFixedValue max = GetCurrentFixedValue(maxAttribute);
            if (value > max)
            {
                value = max;
            }
            else if (value < GASFixedValue.Zero)
            {
                value = GASFixedValue.Zero;
            }
        }

        protected override bool PreProcessInstantEffect(GameplayEffectModCallbackData data)
        {
            var attribute = GetAttribute(data.Modifier.AttributeName);

            if (attribute == Damage)
            {
                float incomingDamage = data.EvaluatedMagnitude;
                if (incomingDamage <= 0) return true;

                float currentHealth = GetCurrentValue(Health);
                float currentDefense = GetCurrentValue(Defense);

                float mitigatedDamage = incomingDamage * (1 - currentDefense / (currentDefense + 100));
                mitigatedDamage = System.Math.Max(0, mitigatedDamage);

                float newHealth = currentHealth - mitigatedDamage;
                newHealth = System.Math.Max(0, newHealth);

                SetBaseValue(Health, newHealth);
                SetCurrentValue(Health, newHealth);

                if (newHealth <= 0 && currentHealth > 0)
                {
                    var targetASC = data.Target;
                    targetASC.AddLooseGameplayTag(GameplayTagManager.Request(GASSampleTags.State_Dead));
                    Log.Warning($"{targetASC.OwnerActor} has died!");
                }

                return true;
            }
            return false;
        }

        public override void PostGameplayEffectExecute(GameplayEffectModCallbackData data)
        {
            base.PostGameplayEffectExecute(data);

            var attribute = GetAttribute(data.Modifier.AttributeName);
            if (attribute == null) return;

            if (attribute == Health)
            {
                SetBaseValue(Health, System.Math.Clamp(GetBaseValue(Health), 0, GetCurrentValue(MaxHealth)));
            }
            else if (attribute == Stamina)
            {
                SetBaseValue(Stamina, System.Math.Clamp(GetBaseValue(Stamina), 0, GetCurrentValue(MaxStamina)));
            }

            if (attribute == Experience)
            {
                if (data.Target.OwnerActor is GASSampleCharacter character)
                {
                    bool hasExpGainTag = data.EffectSpec.Def.AssetTags.HasTag(GameplayTagManager.Request(GASSampleTags.Event_Experience_Gain));
                    if (hasExpGainTag)
                    {
                        character.CheckForLevelUp();
                    }
                    if (character.GetLevelUpData != null)
                    {
                        int maxLevel = character.GetLevelUpData.Levels.Count;
                        int currentLevel = (int)GetBaseValue(Level);

                        if (currentLevel >= maxLevel)
                        {
                            SetBaseValue(Experience, 0);
                        }
                    }
                }
            }
        }
    }
}
