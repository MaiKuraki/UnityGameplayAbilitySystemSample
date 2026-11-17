using System;
using UnityEditor;
using CycloneGames.GameplayAbilities.Editor;
using CycloneGames.GameplayAbilities.Runtime;

namespace GASSample.Editor
{
    [CustomPropertyDrawer(typeof(AttributeNameSelectorAttribute))]
    public class AttributeNameSelectorDrawer : AttributeNameSelectorDrawer_Base
    {
        protected override Type GetConstantsType()
        {
            return typeof(GASSample.Gameplay.GASSampleTags);
        }
    }
}