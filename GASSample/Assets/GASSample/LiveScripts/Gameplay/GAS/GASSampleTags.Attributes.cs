[assembly: CycloneGames.GameplayTags.Runtime.RegisterGameplayTagsFrom(typeof(GASSample.Gameplay.GASSampleTags))]

namespace GASSample.Gameplay
{
    public static partial class GASSampleTags
    {
        public const string Attribute_Primary_Level = "Attribute.Primary.Level";
        public const string Attribute_Primary_Attack = "Attribute.Primary.Attack";
        public const string Attribute_Primary_Defense = "Attribute.Primary.Defense";
        public const string Attribute_Secondary_Health = "Attribute.Secondary.Health";
        public const string Attribute_Secondary_MaxHealth = "Attribute.Secondary.MaxHealth";
        public const string Attribute_Secondary_Stamina = "Attribute.Secondary.Stamina";
        public const string Attribute_Secondary_MaxStamina = "Attribute.Secondary.MaxStamina";
        public const string Attribute_Secondary_MoveSpeed = "Attribute.Secondary.MoveSpeed";
        public const string Attribute_Secondary_MaxMoveSpeed = "Attribute.Secondary.MaxMoveSpeed";
        public const string Attribute_Meta_Experience = "Attribute.Meta.Experience";
        public const string Attribute_Meta_Damage = "Attribute.Meta.Damage";
    }
}