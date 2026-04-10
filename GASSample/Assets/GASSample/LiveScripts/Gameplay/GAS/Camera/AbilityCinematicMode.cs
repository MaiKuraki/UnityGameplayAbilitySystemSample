using CycloneGames.GameplayFramework.Runtime;
using UnityEngine;

namespace GASSample.Gameplay
{
    public sealed class AbilityCinematicMode : CameraMode
    {
        private AbilityCameraClip clip;
        private int currentStageIndex;
        private float stageElapsed;
        private bool finished;

        private Vector3 currentLocalOffset;
        private float currentFov = 60f;
        private Quaternion currentRotationOffset = Quaternion.identity;
        private float blendDuration = 0.15f;

        public bool IsFinished => finished;
        public override float BlendDuration => blendDuration;

        public void Setup(AbilityCameraClip inClip, float inBlendDuration)
        {
            clip = inClip;
            blendDuration = Mathf.Max(0.01f, inBlendDuration);
            currentStageIndex = 0;
            stageElapsed = 0f;
            finished = false;
            currentLocalOffset = Vector3.zero;
            currentFov = 60f;
            currentRotationOffset = Quaternion.identity;
        }

        public bool IsSettled(float positionEpsilon, float fovEpsilon, float targetFov)
        {
            float clampedPosEpsilon = Mathf.Max(0.0001f, positionEpsilon);
            float clampedFovEpsilon = Mathf.Max(0.0001f, fovEpsilon);

            return currentLocalOffset.sqrMagnitude <= clampedPosEpsilon * clampedPosEpsilon
                   && Mathf.Abs(currentFov - targetFov) <= clampedFovEpsilon;
        }

        public override void Tick(CameraContext context, float deltaTime)
        {
            if (clip == null || clip.stages == null || clip.stages.Length == 0 || finished)
            {
                return;
            }

            stageElapsed += deltaTime;

            while (currentStageIndex < clip.stages.Length)
            {
                float stageDuration = Mathf.Max(0f, clip.stages[currentStageIndex].duration);
                if (stageElapsed < stageDuration)
                {
                    break;
                }

                if (stageDuration > 0f)
                {
                    stageElapsed -= stageDuration;
                }
                else
                {
                    stageElapsed = 0f;
                }

                currentStageIndex++;
            }

            if (currentStageIndex >= clip.stages.Length)
            {
                finished = true;
            }
        }

        public override CameraPose Evaluate(CameraContext context, in CameraPose basePose, float deltaTime)
        {
            if (clip == null || clip.stages == null || clip.stages.Length == 0)
            {
                currentLocalOffset = Vector3.zero;
                currentFov = basePose.Fov;
                currentRotationOffset = Quaternion.identity;
                return basePose;
            }

            // Keep the last evaluated cinematic pose until this mode is removed by policy.
            // This avoids a one-frame snap to base pose when the clip just finished.
            if (finished || currentStageIndex >= clip.stages.Length)
            {
                return ComposePose(basePose);
            }

            AbilityCameraClip.CameraStage stage = clip.stages[currentStageIndex];
            float duration = Mathf.Max(0f, stage.duration);
            float t = duration > 0f ? Mathf.Clamp01(stageElapsed / duration) : 1f;

            if (stage.authoringMode == CameraStageAuthoringMode.SimpleFromTo)
            {
                EvaluateFromToStage(stage, stageElapsed, duration, basePose.Fov);
            }
            else
            {
                EvaluateLegacyCurveStage(stage, t, basePose.Fov);
            }

            return ComposePose(basePose);
        }

        private void EvaluateLegacyCurveStage(AbilityCameraClip.CameraStage stage, float t, float baseFov)
        {
            float offsetX = stage.localOffsetX != null ? stage.localOffsetX.Evaluate(t) : 0f;
            float offsetY = stage.localOffsetY != null ? stage.localOffsetY.Evaluate(t) : 0f;
            float offsetZ = stage.localOffsetZ != null ? stage.localOffsetZ.Evaluate(t) : 0f;
            currentLocalOffset = new Vector3(offsetX, offsetY, offsetZ);

            float pitch = stage.pitchOffset != null ? stage.pitchOffset.Evaluate(t) : 0f;
            float yaw = stage.yawOffset != null ? stage.yawOffset.Evaluate(t) : 0f;
            float roll = stage.rollOffset != null ? stage.rollOffset.Evaluate(t) : 0f;
            currentRotationOffset = Quaternion.Euler(pitch, yaw, roll);

            currentFov = stage.fov != null ? stage.fov.Evaluate(t) : baseFov;
        }

        private void EvaluateFromToStage(AbilityCameraClip.CameraStage stage, float elapsed, float duration, float baseFov)
        {
            float posAlpha = EvaluateTimeAlpha(elapsed, duration, stage.positionStartTime, stage.positionEndTime, stage.positionEase);
            float rotAlpha = EvaluateTimeAlpha(elapsed, duration, stage.rotationStartTime, stage.rotationEndTime, stage.rotationEase);
            float fovAlpha = EvaluateTimeAlpha(elapsed, duration, stage.fovStartTime, stage.fovEndTime, stage.fovEase);

            currentLocalOffset = Vector3.LerpUnclamped(stage.positionFrom, stage.positionTo, posAlpha);

            Vector3 euler = Vector3.LerpUnclamped(stage.rotationFrom, stage.rotationTo, rotAlpha);
            currentRotationOffset = Quaternion.Euler(euler);

            float fromFov = stage.useBaseFovAsFrom ? baseFov : stage.fovFrom;
            float toFov = stage.useBaseFovAsTo ? baseFov : stage.fovTo;
            currentFov = Mathf.LerpUnclamped(fromFov, toFov, fovAlpha);
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

        private CameraPose ComposePose(in CameraPose basePose)
        {
            Vector3 worldOffset = basePose.Rotation * currentLocalOffset;
            Vector3 position = basePose.Position + worldOffset;
            Quaternion rotation = basePose.Rotation * currentRotationOffset;
            return new CameraPose(position, rotation, currentFov);
        }
    }
}