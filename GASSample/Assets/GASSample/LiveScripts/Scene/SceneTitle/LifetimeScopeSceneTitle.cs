using VContainer;
using MackySoft.Navigathena.SceneManagement.VContainer;

namespace GASSample.Scene
{
    public class LifetimeScopeSceneTitle : SceneBaseLifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterSceneLifecycle<LifecycleSceneTitle>();
        }
    }
}