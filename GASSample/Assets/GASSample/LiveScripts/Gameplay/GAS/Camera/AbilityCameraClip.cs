using UnityEngine;

namespace GASSample.Gameplay
{
    public enum CameraStageAuthoringMode
    {
        Curve = 0,
        SimpleFromTo = 1
    }

    [CreateAssetMenu(menuName = "GASSample/Ability/AbilityCameraClip", fileName = "AbilityCameraClip")]
    public class AbilityCameraClip : ScriptableObject
    {
        [System.Serializable]
        public struct CameraSample
        {
            public int stageIndex;
            public float stageTime;
            public float stageNormalized;
            public Vector3 localOffset;
            public Vector3 rotationEuler;
            public float fov;
        }

        [System.Serializable]
        public struct CameraStage
        {
            [Min(0f)] public float duration;

            [Header("Authoring")]
            [Tooltip("LegacyCurves keeps the old curve workflow. SimpleFromTo is a value-driven workflow.")]
            public CameraStageAuthoringMode authoringMode;

            [Header("Legacy Curves - Position Offset (Local Space)")]
            public AnimationCurve localOffsetX;
            public AnimationCurve localOffsetY;
            public AnimationCurve localOffsetZ;

            [Header("Rotation Offset (Degrees)")]
            public AnimationCurve pitchOffset;
            public AnimationCurve yawOffset;
            public AnimationCurve rollOffset;

            [Header("Lens")]
            public AnimationCurve fov;

            [Header("FromTo - Position (Local Space)")]
            public Vector3 positionFrom;
            public Vector3 positionTo;
            [Min(0f)] public float positionStartTime;
            [Min(0f)] public float positionEndTime;
            public AnimationCurve positionEase;

            [Header("FromTo - Rotation (Degrees)")]
            public Vector3 rotationFrom;
            public Vector3 rotationTo;
            [Min(0f)] public float rotationStartTime;
            [Min(0f)] public float rotationEndTime;
            public AnimationCurve rotationEase;

            [Header("FromTo - Lens")]
            [Tooltip("Use base camera FOV as blend start value.")]
            public bool useBaseFovAsFrom;
            public float fovFrom;
            [Tooltip("Use base camera FOV as blend target value.")]
            public bool useBaseFovAsTo;
            public float fovTo;
            [Min(0f)] public float fovStartTime;
            [Min(0f)] public float fovEndTime;
            public AnimationCurve fovEase;
        }

        [SerializeField] public CameraStage[] stages = new CameraStage[0];

        private void OnValidate()
        {
            if (stages == null)
            {
                return;
            }

            for (int i = 0; i < stages.Length; i++)
            {
                var stage = stages[i];
                stage.duration = Mathf.Max(0f, stage.duration);

                if (stage.authoringMode == CameraStageAuthoringMode.SimpleFromTo)
                {
                    stage.positionStartTime = Mathf.Clamp(stage.positionStartTime, 0f, stage.duration);
                    stage.positionEndTime = stage.positionEndTime <= 0f ? stage.duration : Mathf.Clamp(stage.positionEndTime, 0f, stage.duration);
                    if (stage.positionEndTime < stage.positionStartTime)
                    {
                        stage.positionEndTime = stage.positionStartTime;
                    }

                    stage.rotationStartTime = Mathf.Clamp(stage.rotationStartTime, 0f, stage.duration);
                    stage.rotationEndTime = stage.rotationEndTime <= 0f ? stage.duration : Mathf.Clamp(stage.rotationEndTime, 0f, stage.duration);
                    if (stage.rotationEndTime < stage.rotationStartTime)
                    {
                        stage.rotationEndTime = stage.rotationStartTime;
                    }

                    stage.fovStartTime = Mathf.Clamp(stage.fovStartTime, 0f, stage.duration);
                    stage.fovEndTime = stage.fovEndTime <= 0f ? stage.duration : Mathf.Clamp(stage.fovEndTime, 0f, stage.duration);
                    if (stage.fovEndTime < stage.fovStartTime)
                    {
                        stage.fovEndTime = stage.fovStartTime;
                    }

                    if (stage.positionEase == null || stage.positionEase.length == 0)
                    {
                        stage.positionEase = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
                    }

                    if (stage.rotationEase == null || stage.rotationEase.length == 0)
                    {
                        stage.rotationEase = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
                    }

                    if (stage.fovEase == null || stage.fovEase.length == 0)
                    {
                        stage.fovEase = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
                    }
                }

                stages[i] = stage;
            }
        }

        public float GetTotalDuration()
        {
            if (stages == null || stages.Length == 0)
            {
                return 0f;
            }

            float total = 0f;
            for (int i = 0; i < stages.Length; i++)
            {
                total += Mathf.Max(0f, stages[i].duration);
            }

            return total;
        }

        public bool TryEvaluateAtTime(float globalTime, float baseFov, out CameraSample sample)
        {
            sample = default;

            if (stages == null || stages.Length == 0)
            {
                return false;
            }

            float remaining = Mathf.Max(0f, globalTime);
            int stageIndex = 0;
            float stageDuration = 0f;

            for (int i = 0; i < stages.Length; i++)
            {
                float duration = Mathf.Max(0f, stages[i].duration);
                if (i == stages.Length - 1 || remaining <= duration)
                {
                    stageIndex = i;
                    stageDuration = duration;
                    break;
                }

                remaining -= duration;
            }

            float stageTime = Mathf.Clamp(remaining, 0f, stageDuration);
            float stageNormalized = stageDuration > 0f ? Mathf.Clamp01(stageTime / stageDuration) : 1f;
            CameraStage stage = stages[stageIndex];

            Vector3 localOffset;
            Vector3 rotationEuler;
            float fov;

            if (stage.authoringMode == CameraStageAuthoringMode.SimpleFromTo)
            {
                float posAlpha = EvaluateTimeAlpha(stageTime, stageDuration, stage.positionStartTime, stage.positionEndTime, stage.positionEase);
                float rotAlpha = EvaluateTimeAlpha(stageTime, stageDuration, stage.rotationStartTime, stage.rotationEndTime, stage.rotationEase);
                float fovAlpha = EvaluateTimeAlpha(stageTime, stageDuration, stage.fovStartTime, stage.fovEndTime, stage.fovEase);

                localOffset = Vector3.LerpUnclamped(stage.positionFrom, stage.positionTo, posAlpha);
                rotationEuler = Vector3.LerpUnclamped(stage.rotationFrom, stage.rotationTo, rotAlpha);

                float fromFov = stage.useBaseFovAsFrom ? baseFov : stage.fovFrom;
                float toFov = stage.useBaseFovAsTo ? baseFov : stage.fovTo;
                fov = Mathf.LerpUnclamped(fromFov, toFov, fovAlpha);
            }
            else
            {
                float offsetX = stage.localOffsetX != null ? stage.localOffsetX.Evaluate(stageNormalized) : 0f;
                float offsetY = stage.localOffsetY != null ? stage.localOffsetY.Evaluate(stageNormalized) : 0f;
                float offsetZ = stage.localOffsetZ != null ? stage.localOffsetZ.Evaluate(stageNormalized) : 0f;
                localOffset = new Vector3(offsetX, offsetY, offsetZ);

                float pitch = stage.pitchOffset != null ? stage.pitchOffset.Evaluate(stageNormalized) : 0f;
                float yaw = stage.yawOffset != null ? stage.yawOffset.Evaluate(stageNormalized) : 0f;
                float roll = stage.rollOffset != null ? stage.rollOffset.Evaluate(stageNormalized) : 0f;
                rotationEuler = new Vector3(pitch, yaw, roll);

                fov = stage.fov != null ? stage.fov.Evaluate(stageNormalized) : baseFov;
            }

            sample = new CameraSample
            {
                stageIndex = stageIndex,
                stageTime = stageTime,
                stageNormalized = stageNormalized,
                localOffset = localOffset,
                rotationEuler = rotationEuler,
                fov = fov
            };

            return true;
        }

        private static float EvaluateTimeAlpha(float elapsed, float stageDuration, float startTime, float endTime, AnimationCurve ease)
        {
            float safeDuration = Mathf.Max(0f, stageDuration);
            float start = Mathf.Clamp(Mathf.Min(startTime, endTime), 0f, safeDuration);
            float end = Mathf.Clamp(Mathf.Max(startTime, endTime), 0f, safeDuration);

            if (Mathf.Approximately(start, end))
            {
                return elapsed >= end ? 1f : 0f;
            }

            float normalized = Mathf.Clamp01((elapsed - start) / (end - start));
            if (ease == null || ease.length == 0)
            {
                return normalized;
            }

            return ease.Evaluate(normalized);
        }
    }
}