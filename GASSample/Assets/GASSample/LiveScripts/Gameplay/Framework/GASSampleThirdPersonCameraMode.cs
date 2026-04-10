using CycloneGames.GameplayFramework.Runtime;
using UnityEngine;

namespace GASSample.Gameplay
{
    public sealed class GASSampleThirdPersonCameraMode : CameraMode
    {
        public float FollowDistance { get; set; } = 4.5f;
        public float PivotHeight { get; set; } = 1.6f;
        public float LookAtHeight { get; set; } = 1.1f;
        public float OverrideFov { get; set; } = 60f;

        public override float BlendDuration => 0.25f;

        public override CameraPose Evaluate(CameraContext context, in CameraPose basePose, float deltaTime)
        {
            Actor target = context?.CurrentViewTarget;
            if (target == null) return basePose;

            target.CalcCamera(deltaTime, out CameraPose targetPose, basePose.Fov);

            Vector3 pivot = targetPose.Position + Vector3.up * PivotHeight;
            Vector3 position = pivot + (targetPose.Rotation * Vector3.back) * FollowDistance;
            Vector3 lookAt = targetPose.Position + Vector3.up * LookAtHeight;
            Quaternion rot = Quaternion.LookRotation((lookAt - position).normalized, Vector3.up);
            float fov = OverrideFov > 0f ? OverrideFov : basePose.Fov;

            return new CameraPose(position, rot, fov);
        }
    }
}