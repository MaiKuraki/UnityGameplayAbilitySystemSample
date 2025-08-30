using System;
using UnityEditor;
using CycloneGames.GameplayAbilities.Editor;
using CycloneGames.GameplayAbilities.Runtime;

namespace GASSample.Editor
{
    /// <summary>
    /// specific implementation of AttributeNameSelectorDrawer_Base for GASSampleTags.
    /// </summary>
    [CustomPropertyDrawer(typeof(AttributeNameSelectorAttribute))]
    public class AttributeNameSelectorDrawer : AttributeNameSelectorDrawer_Base
    {
        /// <summary>
        /// implementation of GetConstantsType to return the type of GASSampleTags.
        /// </summary>
        protected override Type GetConstantsType()
        {
            return typeof(GASSample.Gameplay.GASSampleTags);
        }
    }
}