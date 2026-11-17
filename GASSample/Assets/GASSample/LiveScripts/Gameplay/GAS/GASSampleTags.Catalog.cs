using CycloneGames.GameplayTags.Core;
using UnityEngine;

namespace GASSample.Gameplay
{
    public static partial class GASSampleTags
    {
        /// <summary>
        /// Reflection-free declaration of every tag this sample owns. The previous versions of the
        /// package scanned assemblies for a registration attribute; catalogs are registered explicitly
        /// now so the tags survive IL2CPP stripping and HybridCLR hot updates.
        /// </summary>
        public sealed class Catalog : IGameplayTagCatalog
        {
            public string Name => "GASSample.Tags";

            public void Collect(GameplayTagCatalogBuilder builder)
            {
                builder.Add(Attribute_Primary_Level, Attribute_Primary_Level);
                builder.Add(Attribute_Primary_Attack, Attribute_Primary_Attack);
                builder.Add(Attribute_Primary_Defense, Attribute_Primary_Defense);
                builder.Add(Attribute_Secondary_Health, Attribute_Secondary_Health);
                builder.Add(Attribute_Secondary_MaxHealth, Attribute_Secondary_MaxHealth);
                builder.Add(Attribute_Secondary_Stamina, Attribute_Secondary_Stamina);
                builder.Add(Attribute_Secondary_MaxStamina, Attribute_Secondary_MaxStamina);
                builder.Add(Attribute_Secondary_MoveSpeed, Attribute_Secondary_MoveSpeed);
                builder.Add(Attribute_Secondary_MaxMoveSpeed, Attribute_Secondary_MaxMoveSpeed);
                builder.Add(Attribute_Meta_Experience, Attribute_Meta_Experience);
                builder.Add(Attribute_Meta_Damage, Attribute_Meta_Damage);

                builder.Add(Event_Experience_Gain, Event_Experience_Gain);

                builder.Add(Faction_Player, Faction_Player);
                builder.Add(Faction_NPC_Enemy, Faction_NPC_Enemy);

                builder.Add(Skill_Definition_Attack_1, Skill_Definition_Attack_1);
                builder.Add(Skill_Definition_Attack_2, Skill_Definition_Attack_2);
                builder.Add(Skill_Definition_Attack_3, Skill_Definition_Attack_3);
                builder.Add(Skill_State_ComboWindow, Skill_State_ComboWindow);
                builder.Add(Skill_Cooldown_Attack_1, Skill_Cooldown_Attack_1);
                builder.Add(Skill_Cooldown_Attack_2, Skill_Cooldown_Attack_2);
                builder.Add(Skill_Cooldown_Attack_3, Skill_Cooldown_Attack_3);

                builder.Add(State_Dead, State_Dead);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RegisterCatalogOnLoad()
        {
            GameplayTagManager.RegisterCatalog(new Catalog());
            GameplayTagManager.InitializeIfNeeded();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void RegisterCatalogInEditor()
        {
            GameplayTagManager.RegisterCatalog(new Catalog());
            GameplayTagManager.InitializeIfNeeded();
        }
#endif
    }
}
