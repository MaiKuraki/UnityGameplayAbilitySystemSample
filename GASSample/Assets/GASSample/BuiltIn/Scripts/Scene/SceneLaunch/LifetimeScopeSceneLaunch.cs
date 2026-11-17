using MackySoft.Navigathena.SceneManagement.VContainer;
using VContainer;
using VContainer.Unity;

namespace GASSample.AOT
{
    /// <summary>
    /// Scope for the built-in launch scene. It only registers the lifecycle that pushes Scene_Initial;
    /// everything the game needs is created from there.
    /// </summary>
    public class LifetimeScopeSceneLaunch : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterSceneLifecycle<LifecycleSceneLaunch>();
        }
    }
}
