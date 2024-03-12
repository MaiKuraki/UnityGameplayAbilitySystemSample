using AttributeSystem.Components;
using CycloneGames.GameFramework;
using UnityEngine;

namespace ARPGSample.Gameplay
{
    public class RPGPlayerState : PlayerState
    {
        [SerializeField] private RPGAbilitySystemComponent abilitySystemComponentComponent;
        [SerializeField] private AttributeSystemComponent attributeSystem;
        [SerializeField] private CharacterAttributeSet attributeSet;
        
        public RPGAbilitySystemComponent ASC => abilitySystemComponentComponent;

        protected override void Awake()
        {
            base.Awake();

        }

        protected override void Start()
        {
            base.Start();
            
        }
        
        public float GetHealth()
        {
            if (attributeSystem.GetAttributeValue(attributeSet.Health, out var val))
            {
                return val.CurrentValue;
            }

            return 0;
        }

        public float GetHealthMax()
        {
            if (attributeSystem.GetAttributeValue(attributeSet.HealthMax, out var val))
            {
                return val.CurrentValue;
            }

            return 0;
        }

        public float GetStamina()
        {
            if (attributeSystem.GetAttributeValue(attributeSet.Stamina, out var val))
            {
                return val.CurrentValue;
            }

            return 0;
        }

        public float GetStaminaMax()
        {
            if (attributeSystem.GetAttributeValue(attributeSet.StaminaMax, out var val))
            {
                return val.CurrentValue;
            }

            return 0;
        }

        public float GetMovementSpeed()
        {
            if (attributeSystem.GetAttributeValue(attributeSet.MovementSpeed, out var val))
            {
                return val.CurrentValue;
            }

            return 0;
        }

        public float GetMovementSpeedOrigin()
        {
            if (attributeSystem.GetAttributeValue(attributeSet.MovementSpeedOrigin, out var val))
            {
                return val.CurrentValue;
            }

            return 0;
        }
        
        public float GetJumpForce()
        {
            if (attributeSystem.GetAttributeValue(attributeSet.JumpForce, out var val))
            {
                return val.CurrentValue;
            }

            return 0;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            // abilitySystemComponent.OnAttributeChanged -= OnAttributeChanged;
        }
    }
}