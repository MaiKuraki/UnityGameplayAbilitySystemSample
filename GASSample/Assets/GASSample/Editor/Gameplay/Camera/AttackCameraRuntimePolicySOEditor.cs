using UnityEditor;
using UnityEngine;

namespace GASSample.Gameplay.Editor
{
    [CustomEditor(typeof(AttackCameraRuntimePolicySO))]
    public sealed class AttackCameraRuntimePolicySOEditor : UnityEditor.Editor
    {
        private SerializedProperty basicViewProp;
        private SerializedProperty policyProp;
        private SerializedProperty enableCameraProp;
        private SerializedProperty stopPolicyProp;
        private SerializedProperty blendOutDurationProp;
        private SerializedProperty requireSettledBeforeRemoveProp;
        private SerializedProperty defaultFovProp;
        private SerializedProperty settledPositionEpsilonProp;
        private SerializedProperty settledFovEpsilonProp;
        private SerializedProperty stopCheckIntervalProp;
        private SerializedProperty stopSettleTimeoutProp;

        private bool showAdvanced;

        private void OnEnable()
        {
            basicViewProp = serializedObject.FindProperty("basicView");
            policyProp = serializedObject.FindProperty("policy");
            enableCameraProp = policyProp.FindPropertyRelative("enableCamera");
            stopPolicyProp = policyProp.FindPropertyRelative("stopPolicy");
            blendOutDurationProp = policyProp.FindPropertyRelative("blendOutDuration");
            requireSettledBeforeRemoveProp = policyProp.FindPropertyRelative("requireSettledBeforeRemove");
            defaultFovProp = policyProp.FindPropertyRelative("defaultFov");
            settledPositionEpsilonProp = policyProp.FindPropertyRelative("settledPositionEpsilon");
            settledFovEpsilonProp = policyProp.FindPropertyRelative("settledFovEpsilon");
            stopCheckIntervalProp = policyProp.FindPropertyRelative("stopCheckInterval");
            stopSettleTimeoutProp = policyProp.FindPropertyRelative("stopSettleTimeout");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawScriptField();
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(basicViewProp, new GUIContent("Basic View"));
            bool basicView = basicViewProp.boolValue;

            if (basicView)
            {
                EditorGUILayout.HelpBox(
                    "Basic View shows only essential controls. Use this for onboarding. " +
                    "Disable it to access Advanced Details.",
                    MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "Advanced Details enabled. Full policy control is visible.",
                    MessageType.None);
            }

            EditorGUILayout.HelpBox(
                "This asset controls attack camera lifecycle: when to stop the camera mode and how to blend out. " +
                "Camera path (position/rotation/FOV animation) is configured in AbilityCameraClip.",
                MessageType.Info);

            DrawPresets();
            EditorGUILayout.Space(6f);

            if (basicView)
            {
                DrawBeginnerSection();
            }
            else
            {
                DrawBasicSection();
                EditorGUILayout.Space(6f);
                DrawAdvancedSection();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawScriptField()
        {
            using (new EditorGUI.DisabledScope(true))
            {
                var script = MonoScript.FromScriptableObject((AttackCameraRuntimePolicySO)target);
                EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
            }
        }

        private void DrawPresets()
        {
            EditorGUILayout.LabelField("Quick Presets", EditorStyles.boldLabel);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Beginner Recommended"))
                {
                    ApplyPresetBeginnerRecommended();
                }

                if (GUILayout.Button("Stop By Ability End"))
                {
                    ApplyPresetByAbilityEnd();
                }

                if (GUILayout.Button("Fast Responsive"))
                {
                    ApplyPresetFastResponsive();
                }
            }
        }

        private void DrawBeginnerSection()
        {
            EditorGUILayout.LabelField("Basic Essentials", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableCameraProp, new GUIContent("Enable Camera"));
            EditorGUILayout.PropertyField(stopPolicyProp, new GUIContent("Stop Policy"));
            EditorGUILayout.Slider(blendOutDurationProp, 0.08f, 0.4f, new GUIContent("Blend Out Duration (s)"));

            if (GUILayout.Button("Apply Safe Defaults"))
            {
                ApplyPresetBeginnerRecommended();
            }

            DrawStopPolicyHint();
        }

        private void DrawBasicSection()
        {
            EditorGUILayout.LabelField("Basic", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableCameraProp, new GUIContent("Enable Camera"));
            EditorGUILayout.PropertyField(stopPolicyProp, new GUIContent("Stop Policy"));
            EditorGUILayout.PropertyField(blendOutDurationProp, new GUIContent("Blend Out Duration (s)"));

            DrawStopPolicyHint();
        }

        private void DrawStopPolicyHint()
        {

            var policy = (CameraStopPolicy)stopPolicyProp.enumValueIndex;
            switch (policy)
            {
                case CameraStopPolicy.ByAbilityEnd:
                    EditorGUILayout.HelpBox("Camera is removed when ability ends.", MessageType.None);
                    break;
                case CameraStopPolicy.ByCameraFinished:
                    EditorGUILayout.HelpBox("Camera is removed after camera clip is finished.", MessageType.None);
                    break;
                case CameraStopPolicy.FirstReached:
                    EditorGUILayout.HelpBox("Camera is removed when either ability end or camera finished is reached first.", MessageType.None);
                    break;
            }
        }

        private void DrawAdvancedSection()
        {
            showAdvanced = EditorGUILayout.Foldout(showAdvanced, "Advanced Details", true);
            if (!showAdvanced)
            {
                return;
            }

            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(requireSettledBeforeRemoveProp, new GUIContent("Require Settled Before Remove"));
            EditorGUILayout.PropertyField(defaultFovProp, new GUIContent("Default FOV"));
            EditorGUILayout.PropertyField(settledPositionEpsilonProp, new GUIContent("Settled Position Epsilon"));
            EditorGUILayout.PropertyField(settledFovEpsilonProp, new GUIContent("Settled FOV Epsilon"));
            EditorGUILayout.PropertyField(stopCheckIntervalProp, new GUIContent("Stop Check Interval (s)"));
            EditorGUILayout.PropertyField(stopSettleTimeoutProp, new GUIContent("Stop Settle Timeout (s)"));

            bool settleRelevant = (CameraStopPolicy)stopPolicyProp.enumValueIndex != CameraStopPolicy.ByAbilityEnd;
            if (!settleRelevant)
            {
                EditorGUILayout.HelpBox("Settle-related fields are less relevant when Stop Policy is ByAbilityEnd.", MessageType.Warning);
            }

            EditorGUI.indentLevel--;
        }

        private void ApplyPresetBeginnerRecommended()
        {
            enableCameraProp.boolValue = true;
            stopPolicyProp.enumValueIndex = (int)CameraStopPolicy.ByCameraFinished;
            blendOutDurationProp.floatValue = 0.22f;
            requireSettledBeforeRemoveProp.boolValue = false;
            defaultFovProp.floatValue = 60f;
            settledPositionEpsilonProp.floatValue = 0.02f;
            settledFovEpsilonProp.floatValue = 0.15f;
            stopCheckIntervalProp.floatValue = 0.02f;
            stopSettleTimeoutProp.floatValue = 0.35f;
        }

        private void ApplyPresetByAbilityEnd()
        {
            enableCameraProp.boolValue = true;
            stopPolicyProp.enumValueIndex = (int)CameraStopPolicy.ByAbilityEnd;
            blendOutDurationProp.floatValue = 0.2f;
            requireSettledBeforeRemoveProp.boolValue = false;
        }

        private void ApplyPresetFastResponsive()
        {
            enableCameraProp.boolValue = true;
            stopPolicyProp.enumValueIndex = (int)CameraStopPolicy.FirstReached;
            blendOutDurationProp.floatValue = 0.12f;
            requireSettledBeforeRemoveProp.boolValue = false;
            stopCheckIntervalProp.floatValue = 0.01f;
            stopSettleTimeoutProp.floatValue = 0.2f;
        }
    }
}
