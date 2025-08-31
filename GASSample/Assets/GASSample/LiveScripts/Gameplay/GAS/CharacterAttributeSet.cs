using CycloneGames.GameplayAbilities.Runtime;
using CycloneGames.GameplayTags.Runtime;
using CycloneGames.Logger;

namespace GASSample.Gameplay
{
    public class CharacterAttributeSet : AttributeSet
    {        
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

        public override void PreAttributeChange(GameplayAttribute attribute, ref float newValue)
        {
            base.PreAttributeChange(attribute, ref newValue);

            if (attribute == Health)
            {
                newValue = System.Math.Clamp(newValue, 0.0f, GetCurrentValue(MaxHealth));
            }
            else if (attribute == Stamina)
            {
                newValue = System.Math.Clamp(newValue, 0.0f, GetCurrentValue(MaxStamina));
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

                // --- Death and Bounty Logic ---
                if (newHealth <= 0 && currentHealth > 0)
                {
                    var targetASC = data.Target;
                    targetASC.AddLooseGameplayTag(GameplayTagManager.RequestTag(GASSampleTags.State_Dead));
                    CLogger.LogWarning($"{targetASC.OwnerActor} has died!");

                    var killerASC = data.EffectSpec.Source;
                    if (killerASC != null && killerASC != targetASC)
                    {
                        if (targetASC.OwnerActor is GASSampleCharacter deadCharacter)
                        {
                            // deadCharacter.GrantBountyTo(killerASC);
                        }
                    }
                }

                return true; // Handled, no further processing needed.
            }
            return false; // For any other attribute, return false to allow the default logic to run.
        }

        protected override void PostProcessInstantEffect(GameplayEffectModCallbackData data)
        {
            base.PostProcessInstantEffect(data);

            var attribute = GetAttribute(data.Modifier.AttributeName);
            if (attribute == Experience)
            {
                if (data.Target.OwnerActor is GASSampleCharacter character)
                {
                    // character.CheckForLevelUp();
                }
            }
        }
    }
}
