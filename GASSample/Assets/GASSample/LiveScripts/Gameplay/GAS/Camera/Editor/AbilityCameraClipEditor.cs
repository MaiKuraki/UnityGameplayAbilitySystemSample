using UnityEditor;
using UnityEngine;

namespace GASSample.Gameplay.Editor
{
    [CustomEditor(typeof(AbilityCameraClip))]
    public sealed class AbilityCameraClipEditor : UnityEditor.Editor
    {
        private SerializedProperty stagesProp;
        private float previewTime;
        private float previewBaseFov = 60f;

        private void OnEnable()
        {
            stagesProp = serializedObject.FindProperty("stages");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawScriptField();
            EditorGUILayout.Space();

            DrawStages();
            EditorGUILayout.Space(8f);
            DrawPreview();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawScriptField()
        {
            using (new EditorGUI.DisabledScope(true))
            {
                MonoScript script = MonoScript.FromScriptableObject((AbilityCameraClip)target);
                EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
            }
        }

        private void DrawStages()
        {
            if (stagesProp == null)
            {
                EditorGUILayout.HelpBox("Stages property not found.", MessageType.Error);
                return;
            }

            EditorGUILayout.PropertyField(stagesProp.FindPropertyRelative("Array.size"), new GUIContent("Stage Count"));

            for (int i = 0; i < stagesProp.arraySize; i++)
            {
                SerializedProperty stageProp = stagesProp.GetArrayElementAtIndex(i);
                stageProp.isExpanded = EditorGUILayout.Foldout(stageProp.isExpanded, $"Stage {i}", true);
                if (!stageProp.isExpanded)
                {
                    continue;
                }

                using (new EditorGUILayout.VerticalScope("box"))
                {
                    DrawStage(stageProp);
                }
            }
        }

        private static void DrawStage(SerializedProperty stage)
        {
            EditorGUILayout.PropertyField(stage.FindPropertyRelative("duration"), new GUIContent("Duration (Seconds)"));
            EditorGUILayout.PropertyField(stage.FindPropertyRelative("authoringMode"), new GUIContent("Authoring Mode"));

            CameraStageAuthoringMode mode = (CameraStageAuthoringMode)stage.FindPropertyRelative("authoringMode").enumValueIndex;
            EditorGUILayout.Space(4f);

            if (mode == CameraStageAuthoringMode.Curve)
            {
                DrawLegacy(stage);
            }
            else
            {
                DrawSimpleFromTo(stage);
            }
        }

        private static void DrawLegacy(SerializedProperty stage)
        {
            EditorGUILayout.LabelField("Legacy Curves", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(stage.FindPropertyRelative("localOffsetX"), new GUIContent("Local Offset X"));
            EditorGUILayout.PropertyField(stage.FindPropertyRelative("localOffsetY"), new GUIContent("Local Offset Y"));
            EditorGUILayout.PropertyField(stage.FindPropertyRelative("localOffsetZ"), new GUIContent("Local Offset Z"));
            EditorGUILayout.PropertyField(stage.FindPropertyRelative("pitchOffset"), new GUIContent("Pitch Offset"));
            EditorGUILayout.PropertyField(stage.FindPropertyRelative("yawOffset"), new GUIContent("Yaw Offset"));
            EditorGUILayout.PropertyField(stage.FindPropertyRelative("rollOffset"), new GUIContent("Roll Offset (Z)"));
            EditorGUILayout.PropertyField(stage.FindPropertyRelative("fov"), new GUIContent("FOV"));
        }

        private static void DrawSimpleFromTo(SerializedProperty stage)
        {
            EditorGUILayout.LabelField("Simple From/To", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("Position (Local)", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(stage.FindPropertyRelative("positionFrom"), new GUIContent("From"));
            EditorGUILayout.PropertyField(stage.FindPropertyRelative("positionTo"), new GUIContent("To"));
            EditorGUILayout.PropertyField(stage.FindPropertyRelative("positionStartTime"), new GUIContent("Start Time (s)"));
            EditorGUILayout.PropertyField(stage.FindPropertyRelative("positionEndTime"), new GUIContent("End Time (s)"));
            EditorGUILayout.PropertyField(stage.FindPropertyRelative("positionEase"), new GUIContent("Ease"));

            EditorGUILayout.Space(3f);
            EditorGUILayout.LabelField("Rotation (Degrees)", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(stage.FindPropertyRelative("rotationFrom"), new GUIContent("From (Pitch/Yaw/Roll)"));
            EditorGUILayout.PropertyField(stage.FindPropertyRelative("rotationTo"), new GUIContent("To (Pitch/Yaw/Roll)"));
            EditorGUILayout.PropertyField(stage.FindPropertyRelative("rotationStartTime"), new GUIContent("Start Time (s)"));
            EditorGUILayout.PropertyField(stage.FindPropertyRelative("rotationEndTime"), new GUIContent("End Time (s)"));
            EditorGUILayout.PropertyField(stage.FindPropertyRelative("rotationEase"), new GUIContent("Ease"));

            EditorGUILayout.Space(3f);
            EditorGUILayout.LabelField("Lens", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(stage.FindPropertyRelative("useBaseFovAsFrom"), new GUIContent("From = Base FOV"));
            if (!stage.FindPropertyRelative("useBaseFovAsFrom").boolValue)
            {
                EditorGUILayout.PropertyField(stage.FindPropertyRelative("fovFrom"), new GUIContent("FOV From"));
            }

            EditorGUILayout.PropertyField(stage.FindPropertyRelative("useBaseFovAsTo"), new GUIContent("To = Base FOV"));
            if (!stage.FindPropertyRelative("useBaseFovAsTo").boolValue)
            {
                EditorGUILayout.PropertyField(stage.FindPropertyRelative("fovTo"), new GUIContent("FOV To"));
            }

            EditorGUILayout.PropertyField(stage.FindPropertyRelative("fovStartTime"), new GUIContent("Start Time (s)"));
            EditorGUILayout.PropertyField(stage.FindPropertyRelative("fovEndTime"), new GUIContent("End Time (s)"));
            EditorGUILayout.PropertyField(stage.FindPropertyRelative("fovEase"), new GUIContent("Ease"));

            EditorGUILayout.HelpBox(
                "Tip: For 'zoom in then roll around Z', set rotation roll to start later than position by using different Start Time values.",
                MessageType.Info);
        }

        private void DrawPreview()
        {
            var clip = (AbilityCameraClip)target;
            float totalDuration = clip.GetTotalDuration();

            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
            if (totalDuration <= 0f)
            {
                EditorGUILayout.HelpBox("No stage duration found. Set at least one stage duration > 0 to preview values.", MessageType.Info);
                return;
            }

            previewBaseFov = EditorGUILayout.FloatField("Base FOV", previewBaseFov);
            previewTime = EditorGUILayout.Slider("Time", previewTime, 0f, totalDuration);

            if (!clip.TryEvaluateAtTime(previewTime, previewBaseFov, out var sample))
            {
                EditorGUILayout.HelpBox("Failed to evaluate camera clip at current time.", MessageType.Warning);
                return;
            }

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.IntField("Stage Index", sample.stageIndex);
                EditorGUILayout.FloatField("Stage Time (s)", sample.stageTime);
                EditorGUILayout.Slider("Stage Progress", sample.stageNormalized, 0f, 1f);
                EditorGUILayout.Vector3Field("Local Offset (m)", sample.localOffset);
                EditorGUILayout.Vector3Field("Rotation Offset (deg)", sample.rotationEuler);
                EditorGUILayout.FloatField("FOV (deg)", sample.fov);
            }
        }
    }
}
