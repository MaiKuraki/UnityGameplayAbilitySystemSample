using AttributeSystem.Authoring;
using UnityEngine;

namespace ARPGSample.Gameplay
{
    [CreateAssetMenu(menuName = "CycloneGames/GAS/CharacterAttributeSet")]
    public class CharacterAttributeSet : ScriptableObject
    {
        [SerializeField] private AttributeScriptableObject health;
        [SerializeField] private AttributeScriptableObject healthMax;
        [SerializeField] private AttributeScriptableObject stamina;
        [SerializeField] private AttributeScriptableObject staminaMax;
        [SerializeField] private AttributeScriptableObject movementSpeed;
        [SerializeField] private AttributeScriptableObject movementSpeedOrigin;
        [SerializeField] private AttributeScriptableObject jumpForce;

        public AttributeScriptableObject Health => health;
        public AttributeScriptableObject HealthMax => healthMax;
        public AttributeScriptableObject Stamina => stamina;
        public AttributeScriptableObject StaminaMax => staminaMax;
        public AttributeScriptableObject MovementSpeed => movementSpeed;
        public AttributeScriptableObject MovementSpeedOrigin => movementSpeedOrigin;
        public AttributeScriptableObject JumpForce => jumpForce;
    }
}