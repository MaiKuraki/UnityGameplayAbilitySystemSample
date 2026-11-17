using UnityEngine;

namespace GASSample.Gameplay
{
    /// <summary>Position, rotation and field of view of the gameplay camera for one frame.</summary>
    public readonly struct CameraPose
    {
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;
        public readonly float Fov;

        public CameraPose(Vector3 position, Quaternion rotation, float fov)
        {
            Position = position;
            Rotation = rotation;
            Fov = fov;
        }
    }
}
