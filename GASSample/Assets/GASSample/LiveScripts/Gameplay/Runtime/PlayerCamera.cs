using System.Collections.Generic;
using UnityEngine;

namespace GASSample.Gameplay
{
    /// <summary>
    /// Third-person follow camera. The base pose sits behind <see cref="SetTarget"/>; the top mode in
    /// the stack overrides it and blends in and out over the mode's own duration.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class PlayerCamera : MonoBehaviour, IAbilityCamera
    {
        [SerializeField] private float followDistance = 3f;
        [SerializeField] private float pivotHeight = 1.6f;
        [SerializeField] private float lookAtHeight = 1.1f;
        [SerializeField] private float fieldOfView = 60f;

        private readonly List<CameraMode> modes = new List<CameraMode>();

        private Camera cameraComponent;
        private Transform target;
        private CameraMode blendingOutMode;
        private float modeBlend;

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        public bool TryPushMode(CameraMode mode)
        {
            if (mode == null || modes.Contains(mode))
            {
                return false;
            }

            modes.Add(mode);
            return true;
        }

        public bool RemoveMode(CameraMode mode)
        {
            int index = modes.LastIndexOf(mode);
            if (index < 0)
            {
                return false;
            }

            modes.RemoveAt(index);
            return true;
        }

        private void Awake()
        {
            cameraComponent = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            float deltaTime = Time.deltaTime;
            CameraPose basePose = EvaluateBasePose();
            CameraMode mode = modes.Count > 0 ? modes[modes.Count - 1] : null;

            if (mode != null)
            {
                mode.Tick(deltaTime);
                modeBlend = MoveTowardsBlend(modeBlend, 1f, mode.BlendDuration, deltaTime);
                blendingOutMode = mode;
                Apply(Blend(basePose, mode.Evaluate(basePose), modeBlend));
                return;
            }

            if (blendingOutMode != null && modeBlend > 0f)
            {
                modeBlend = MoveTowardsBlend(modeBlend, 0f, blendingOutMode.BlendDuration, deltaTime);
                Apply(Blend(basePose, blendingOutMode.Evaluate(basePose), modeBlend));
                if (modeBlend <= 0f)
                {
                    blendingOutMode = null;
                }

                return;
            }

            Apply(basePose);
        }

        private CameraPose EvaluateBasePose()
        {
            if (target == null)
            {
                return new CameraPose(transform.position, transform.rotation, fieldOfView);
            }

            Vector3 pivot = target.position + Vector3.up * pivotHeight;
            Vector3 position = pivot - target.forward * followDistance;
            Vector3 lookAt = target.position + Vector3.up * lookAtHeight;
            Quaternion rotation = Quaternion.LookRotation(lookAt - position, Vector3.up);
            return new CameraPose(position, rotation, fieldOfView);
        }

        private void Apply(in CameraPose pose)
        {
            transform.SetPositionAndRotation(pose.Position, pose.Rotation);
            cameraComponent.fieldOfView = pose.Fov;
        }

        private static float MoveTowardsBlend(float current, float target, float duration, float deltaTime)
        {
            float step = deltaTime / Mathf.Max(0.01f, duration);
            return Mathf.MoveTowards(current, target, step);
        }

        private static CameraPose Blend(in CameraPose from, in CameraPose to, float t)
        {
            return new CameraPose(
                Vector3.Lerp(from.Position, to.Position, t),
                Quaternion.Slerp(from.Rotation, to.Rotation, t),
                Mathf.Lerp(from.Fov, to.Fov, t));
        }
    }
}
