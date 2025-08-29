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
        public const string Attribute_Secondary_Mana = "Attribute.Secondary.Mana";
        public const string Attribute_Secondary_MaxMana = "Attribute.Secondary.MaxMana";
        public const string Attribute_Meta_Experience = "Attribute.Meta.Experience";
        public const string Attribute_Meta_Damage = "Attribute.Meta.Damage";
    }
}