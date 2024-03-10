using System.Collections.Generic;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using UnityEngine;

namespace ARPGSample.Gameplay
{
    [CreateAssetMenu(menuName = "CycloneGames/GAS/FixedValueAttribute")]
    public class FixedValueAttribute : AttributeScriptableObject
    {
        [SerializeField] private float fixedValue;

        public override AttributeValue CalculateInitialValue(AttributeValue attributeValue, List<AttributeValue> otherAttributeValues)
        {
            attributeValue.BaseValue = fixedValue;
            attributeValue.CurrentValue = fixedValue;
            
            return attributeValue;
        }

        public override AttributeValue CalculateCurrentAttributeValue(AttributeValue attributeValue, List<AttributeValue> otherAttributeValues)
        {
            attributeValue.BaseValue = fixedValue;
            attributeValue.CurrentValue = fixedValue;
            
            return attributeValue;
        }
    }
}