using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GASSample.Gameplay
{
    public enum CameraStopPolicy
    {
        ByAbilityEnd,
        ByCameraFinished,
        FirstReached
    }

    [Serializable]
    public struct AttackCameraRuntimePolicy
    {
        [Tooltip("Enable or disable attack camera mode.")]
        public bool enableCamera;

        [Tooltip("When to remove the attack camera mode.")]
        public CameraStopPolicy stopPolicy;

        [Tooltip("How long it takes to blend from attack camera back to base camera.")]
        [Min(0.01f)]
        public float blendOutDuration;

        [Tooltip("When true, wait until camera offset/FOV are considered settled before removing mode.")]
        public bool requireSettledBeforeRemove;

        [Tooltip("Reference FOV used by settled checks.")]
        [Min(1f)]
        public float defaultFov;

        [Tooltip("Position epsilon for settled check. Lower value means stricter settle requirement.")]
        [Min(0.0001f)]
        public float settledPositionEpsilon;

        [Tooltip("FOV epsilon for settled check. Lower value means stricter settle requirement.")]
        [Min(0.0001f)]
        public float settledFovEpsilon;

        [Tooltip("How often to poll settle state while waiting for camera stop.")]
        [Min(0.01f)]
        public float stopCheckInterval;

        [Tooltip("Maximum wait time for settle check before forcing camera stop.")]
        [Min(0.01f)]
        public float stopSettleTimeout;

        public static AttackCameraRuntimePolicy Default => new AttackCameraRuntimePolicy
        {
            enableCamera = true,
            stopPolicy = CameraStopPolicy.ByCameraFinished,
            blendOutDuration = 0.15f,
            requireSettledBeforeRemove = true,
            defaultFov = 60f,
            settledPositionEpsilon = 0.02f,
            settledFovEpsilon = 0.15f,
            stopCheckInterval = 0.02f,
            stopSettleTimeout = 0.35f
        };
    }

    [CreateAssetMenu(
        fileName = "AttackCameraRuntimePolicy",
        menuName = "GASSample/Camera/Attack Runtime Policy")]
    public class AttackCameraRuntimePolicySO : ScriptableObject
    {
        [FormerlySerializedAs("beginnerMode")]
        [SerializeField] private bool basicView = true;
        [SerializeField] private AttackCameraRuntimePolicy policy = AttackCameraRuntimePolicy.Default;

        public bool BasicView => basicView;
        public AttackCameraRuntimePolicy Value => policy;
    }
}