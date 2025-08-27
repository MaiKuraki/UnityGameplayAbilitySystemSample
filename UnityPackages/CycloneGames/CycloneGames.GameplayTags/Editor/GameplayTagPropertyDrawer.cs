﻿using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using CycloneGames.GameplayTags.Runtime;

namespace CycloneGames.GameplayTags.Editor
{
   [CustomPropertyDrawer(typeof(GameplayTag))]
   public class GameplayTagPropertyDrawer : PropertyDrawer
   {
      private static readonly GUIContent s_TempContent = new();

      public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
      {
         label = EditorGUI.BeginProperty(position, label, property);
         position = EditorGUI.PrefixLabel(position, label);

         int oldIndentLevel = EditorGUI.indentLevel;
         EditorGUI.indentLevel = 0;

         SerializedProperty nameProperty = property.FindPropertyRelative("m_Name");
         
         if (nameProperty == null)
         {
            EditorGUI.LabelField(position, label, new GUIContent("Invalid Tag Property"));
            EditorGUI.EndProperty();
            return;
         }

         GameplayTag tag = GameplayTagManager.RequestTag(nameProperty.stringValue);

         if (tag != GameplayTag.None)
         {
            s_TempContent.text = tag.Name;
            s_TempContent.tooltip = tag.Description;
         }
         else
         {
            s_TempContent.text = "Select...";
         }

         if (EditorGUI.DropdownButton(position, s_TempContent, FocusType.Keyboard))
         {
            GameplayTagTreeView tagTreeView = new(new TreeViewState(), property, static () =>
            {
               EditorApplication.delayCall += () =>
                   {
                      if (EditorWindow.HasOpenInstances<PopupWindow>())
                      {
                         EditorWindow.GetWindow<PopupWindow>().Close();
                      }
                   };
            });

            TreeViewMethodExtensions.ShowPopupWindow(tagTreeView, position, 280f);
         }

         EditorGUI.indentLevel = oldIndentLevel;
         EditorGUI.EndProperty();
      }
   }
}