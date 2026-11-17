namespace GASSample.Gameplay
{
    /// <summary>
    /// A camera behaviour that can be pushed on top of the follow camera, for example the cinematic
    /// clip an ability plays while it runs.
    /// </summary>
    public abstract class CameraMode
    {
        /// <summary>Seconds the camera takes to blend into and out of this mode.</summary>
        public virtual float BlendDuration => 0.2f;

        public virtual void Tick(float deltaTime)
        {
        }

        public abstract CameraPose Evaluate(in CameraPose basePose);
    }
}
