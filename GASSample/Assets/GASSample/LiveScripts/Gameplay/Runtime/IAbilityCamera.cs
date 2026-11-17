namespace GASSample.Gameplay
{
    /// <summary>Camera handle an ability uses to own the view while it is active.</summary>
    public interface IAbilityCamera
    {
        bool TryPushMode(CameraMode mode);
        bool RemoveMode(CameraMode mode);
    }
}
